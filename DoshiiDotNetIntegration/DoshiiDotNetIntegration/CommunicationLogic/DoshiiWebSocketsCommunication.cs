﻿using AutoMapper;
using DoshiiDotNetIntegration.Models.Json;
using System;
using System.Threading;
using WebSocketSharp;

namespace DoshiiDotNetIntegration.CommunicationLogic
{
    /// <summary>
    /// DO NOT USE, This class is used internally by the SDK and should not be instantiated by the POS.
    /// facilitates the web sockets communication with the doshii application
    /// </summary>
    internal class DoshiiWebSocketsCommunication : IDisposable
    {

        #region fields and properties

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Web socket object that will handle all the communications with doshii
        /// </summary>
		internal WebSocket m_WebSocketsConnection { get; private set; }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// A doshii logic instance
        /// </summary>
        internal DoshiiManager m_DoshiiLogic { get; private set; }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// A thread to send heartbeat socket message to doshii. 
        /// </summary>
        private Thread m_HeartBeatThread = null;

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// This will hold the value for the last connected time so that there are not many connections established after the connection drops out. 
        /// </summary>
        private DateTime m_LastConnectionAttemptTime = DateTime.MinValue;

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// this is used to hold the timeout value for the socket connection being unable to connect to the server.
        /// </summary>
        internal int m_SocketConnectionTimeOutValue { get; private set; }

        /// <summary>
        /// this is used to calculate if the last successful socket connection is within the timeOut range. 
        /// </summary>
        internal DateTime m_LastSuccessfullSocketMessageTime { get; private set; }

		/// <summary>
		/// Callback to the POS logging mechanism.
		/// </summary>
		internal DoshiiLogManager mLog { get; private set; }

		/// <summary>
		/// Ping message to send during heartbeat checks.
		/// </summary>
		private const string PingMessage = "primus::pong::";

        #endregion

        #region internal methods and events

        #region events

		// TODO: [BC]: Consider making the following events public to expose them to the POS implementation.
        
        internal delegate void OrderStatusEventHandler(object sender, CommunicationEventArgs.OrderEventArgs e);
        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Event will be raised when the state of an order has changed through doshii
        /// </summary>
        internal event OrderStatusEventHandler OrderStatusEvent;
        
        internal delegate void SocketCommunicationEstablishedEventHandler(object sender, EventArgs e);
        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Event will be raised when the socket communication with doshii are opened. 
        /// </summary>
        internal event SocketCommunicationEstablishedEventHandler SocketCommunicationEstablishedEvent;

        internal delegate void SocketCommunicationTimeoutReachedEventHandler(object sender, EventArgs e);
        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Event will be raised when the socket communication with doshii are opened. 
        /// </summary>
        internal event SocketCommunicationTimeoutReachedEventHandler SocketCommunicationTimeoutReached;

        #endregion

        #region methods

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Closes the socket connection
        /// </summary>
        internal virtual void CloseSocketConnection()
        {
            m_WebSocketsConnection.Close();
        }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// constructor, 
        /// The initialize method must be called after the constructor to initialize the connection
        /// </summary>
        /// <param name="webSocketUrl"></param>
		/// <param name="socketConnectionTimeOutValue"></param>
        /// <param name="doshii"></param>
		/// <param name="logManager"></param>
        internal DoshiiWebSocketsCommunication(string webSocketUrl, int socketConnectionTimeOutValue, DoshiiLogManager logManager, DoshiiManager doshii)
        {
            if (doshii == null)
            {
                throw new ArgumentNullException("doshii");
            }

			if (logManager == null)
			{
				throw new ArgumentNullException("logManager");
			}

			m_SocketConnectionTimeOutValue = socketConnectionTimeOutValue;
			mLog = logManager;
			m_DoshiiLogic = doshii;

            if (socketConnectionTimeOutValue < 10)
            {
				mLog.LogMessage(typeof(DoshiiWebSocketsCommunication), Enums.DoshiiLogLevels.Error, string.Format("The socketConnectionTimeOutValue is set to less than 10 seconds, Times smaller than 10 seconds are not supported."));
				throw new ArgumentException("SocketConnectionTimeOutValue is less than 10");
            }

			mLog.LogMessage(typeof(DoshiiWebSocketsCommunication), Enums.DoshiiLogLevels.Debug, string.Format("instantiating doshiiwebSocketComunication with socketUrl - '{0}'", webSocketUrl));
                
            if (string.IsNullOrWhiteSpace(webSocketUrl))
            {
				mLog.LogMessage(typeof(DoshiiWebSocketsCommunication), Enums.DoshiiLogLevels.Error, string.Format("cannot create an instance of DoshiiWebSocketsCommunication with a blank socketUrl"));
				throw new ArgumentException("webSocketUrl");
            }
                        
            m_WebSocketsConnection = new WebSocket(webSocketUrl);
            m_WebSocketsConnection.OnOpen += new EventHandler(WebSocketsConnectionOnOpenEventHandler);
            m_WebSocketsConnection.OnClose += new EventHandler<CloseEventArgs>(WebSocketsConnectionOnCloseEventHandler);
            m_WebSocketsConnection.OnMessage += new EventHandler<MessageEventArgs>(WebSocketsConnectionOnMessageEventHandler);
            m_WebSocketsConnection.OnError += new EventHandler<ErrorEventArgs>(WebSocketsConnectionOnErrorEventHandler);
        }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Initializes the webScoket communication with doshii
        /// This method should not be called until the events emitted from this class are subscribed to. 
        /// </summary>
        internal virtual void Initialize()
        {
			mLog.LogMessage(typeof(DoshiiWebSocketsCommunication), Enums.DoshiiLogLevels.Debug, string.Format("initializing doshii web-socket connection"));
            Connect();
        }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Starts the heart beat thread to ensure the sockets connection is kept alive. 
        /// </summary>
        internal virtual void StartHeartbeatThread()
        {
            if (m_HeartBeatThread == null)
            {
                m_HeartBeatThread = new Thread(new ThreadStart(HeartBeatChecker));
                m_HeartBeatThread.IsBackground = true;
            }
            if (!m_HeartBeatThread.IsAlive)
            {
                m_HeartBeatThread.Start();
            }
            
        }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Sets the last time a connection attempt was made. 
        /// </summary>
        internal virtual void SetLastConnectionAttemptTime()
        {
			mLog.LogMessage(typeof(DoshiiWebSocketsCommunication), Enums.DoshiiLogLevels.Debug, string.Format("Doshii: Setting last connection attempt time: {0}", DateTime.Now.ToString())); 
            m_LastConnectionAttemptTime = DateTime.Now;
        }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Attempts to open a web-sockets connection with doshii
        /// </summary>
        internal virtual void Connect()
        {
            if (m_WebSocketsConnection != null && !m_WebSocketsConnection.IsAlive)
            {
                try
                {
                    if ((DateTime.Now.AddSeconds(-10) > m_LastConnectionAttemptTime))
                    {
                        SetLastConnectionAttemptTime();
						mLog.LogMessage(typeof(DoshiiWebSocketsCommunication), Enums.DoshiiLogLevels.Debug, string.Format("Doshii: Attempting Socket connection")); 
                        m_WebSocketsConnection.Connect();
                    }
                    
                }
                catch(Exception ex)
                {
					mLog.LogMessage(typeof(DoshiiWebSocketsCommunication), Enums.DoshiiLogLevels.Error, string.Format("Doshii: There was an error initializing the web sockets connection to Doshii"), ex);
                }
                
            }
            else
            {
				mLog.LogMessage(typeof(DoshiiWebSocketsCommunication), Enums.DoshiiLogLevels.Error, string.Format("Doshii: Attempted to open a web socket connection before initializing the was object"));
            }
        }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// This should not be used for anything other than heartbeats as the POS does not communication with Doshii though web sockets. 
        /// </summary>
        /// <param name="message"></param>
		/// <returns>True on successful send; false otherwise.</returns>
        internal virtual bool SendMessage(string message)
        {
            try
            {
				mLog.LogMessage(typeof(DoshiiWebSocketsCommunication), Enums.DoshiiLogLevels.Debug, string.Format("Doshii: Sending web-sockets message '{0}' to {1}", message, m_WebSocketsConnection.Url.ToString()));
                if (m_WebSocketsConnection.IsAlive)
                {
                    m_WebSocketsConnection.Send(message);
					return true;
                }
            }
            catch (Exception ex)
            {
				mLog.LogMessage(typeof(DoshiiWebSocketsCommunication), Enums.DoshiiLogLevels.Error, string.Format("Doshii: Exception while sending a webSocket message '{0}' to {1}", message, m_WebSocketsConnection.Url.ToString()), ex);
            }

			return false;
        }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Sends a heart beat message to doshii every ten seconds. 
        /// </summary>
        internal virtual void HeartBeatChecker()
        {
			mLog.LogMessage(typeof(DoshiiWebSocketsCommunication), Enums.DoshiiLogLevels.Debug, string.Format("Starting web Socket heartbeat thread"));
            while (true)
            {
                Thread.Sleep(10000);
                if (!TestTimeOutValue())
                {
                    //raise event to signal that Timeout out is expired.
                    SocketCommunicationTimeoutReached(this, new EventArgs());
                }

                if (m_WebSocketsConnection.IsAlive)
                {
                    TimeSpan thisTimeSpan = new TimeSpan(DateTime.UtcNow.Ticks);
                    double doubleForHeartbeat = thisTimeSpan.TotalMilliseconds;
                    string message = string.Format("\"{0}<{1}>\"", DoshiiWebSocketsCommunication.PingMessage, doubleForHeartbeat.ToString());
					if (SendMessage(message))
						SetLastSuccessfullSocketCommunicationTime();
                }
                else
                {
                    Initialize();
                }
            }
        }

        #endregion

        #endregion

        #region event handlers

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Handles the webSocket onError event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal virtual void WebSocketsConnectionOnErrorEventHandler(object sender, ErrorEventArgs e)
        {
			mLog.LogMessage(typeof(DoshiiWebSocketsCommunication), Enums.DoshiiLogLevels.Error, string.Format("Doshii: There was an error with the web-sockets connection to {0} the error was {1}", m_WebSocketsConnection.Url.ToString(), e.Message));
            //StopHeartbeatThread();
            if (e.Message == "The WebSocket connection has already been closed.")
            {
                Initialize();
            }
        }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Handles the webSocket onMessage event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal virtual void WebSocketsConnectionOnMessageEventHandler(object sender, MessageEventArgs e)
        {
            SetLastSuccessfullSocketCommunicationTime();
            SocketMessage theMessage = new SocketMessage();
            if (e.Type == Opcode.Text)
            {
                string messageString = e.Data.ToString();
                // drop heart beat response
                if (messageString.Contains(String.Format("{0}<", DoshiiWebSocketsCommunication.PingMessage)))
                {
					mLog.LogMessage(typeof(DoshiiWebSocketsCommunication), Enums.DoshiiLogLevels.Debug, string.Format("Doshii: Received web-sockets message '{0}' from {1}", messageString, m_WebSocketsConnection.Url.ToString()));
                    return;
                }
                else
                {
                    theMessage = SocketMessage.deseralizeFromJson(messageString);
                }
                
            }

            if (e.Type == Opcode.Binary)
            {
                string messageString = e.RawData.ToString();
                // drop heartbeat message
                if (messageString.Contains(String.Format("{0}<", DoshiiWebSocketsCommunication.PingMessage)))
                {
                    return;
                }
                else
                {
                    theMessage = SocketMessage.deseralizeFromJson(messageString);
                }
                
            }
			mLog.LogMessage(typeof(DoshiiWebSocketsCommunication), Enums.DoshiiLogLevels.Debug, string.Format("WebScoket message received - '{0}'", theMessage.ToString()));
            dynamic dynamicSocketMessageData = theMessage.Emit[1];

            SocketMessageData messageData = new SocketMessageData();

            messageData.EventName = (string)theMessage.Emit[0];
            messageData.CheckinId = (string)dynamicSocketMessageData.checkinId;
            messageData.OrderId = (string)dynamicSocketMessageData.orderId;
            messageData.meerkatConsumerId = (string)dynamicSocketMessageData.meerkatConsumerId;
            messageData.Status = (string)dynamicSocketMessageData.status;
            messageData.Name = (string)dynamicSocketMessageData.name;
            messageData.Id = (string)dynamicSocketMessageData.id;
            
            string uriString = (string)dynamicSocketMessageData.uri;
            if (!string.IsNullOrWhiteSpace(uriString))
            {
                messageData.Uri = new Uri((string)dynamicSocketMessageData.uri);
            }
            
            switch (messageData.EventName)
            {
                case "order_status":
                    CommunicationEventArgs.OrderEventArgs orderStatusEventArgs = new CommunicationEventArgs.OrderEventArgs();
                    orderStatusEventArgs.Order = Mapper.Map<JsonOrder>(m_DoshiiLogic.GetOrder(messageData.OrderId));
                    orderStatusEventArgs.OrderId = messageData.OrderId;
                    orderStatusEventArgs.Status = messageData.Status;    
                
                    OrderStatusEvent(this, orderStatusEventArgs);
                    break;
                default:
					mLog.LogMessage(typeof(DoshiiWebSocketsCommunication), Enums.DoshiiLogLevels.Warning, string.Format("Doshii: Received socket message is not a supported message. messageType - '{0}'", messageData.EventName));
                    break;
            }
            
        }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Handles the webSocket onClose event, 
        /// logs that the web-sockets connection to Doshii is no longer active. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal virtual void WebSocketsConnectionOnCloseEventHandler(object sender, CloseEventArgs e)
        {
			mLog.LogMessage(typeof(DoshiiWebSocketsCommunication), Enums.DoshiiLogLevels.Debug, string.Format("Doshii: WebScokets connection to {0} closed", m_WebSocketsConnection.Url.ToString()));
        }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Handles the webScoket onOpen event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal virtual void WebSocketsConnectionOnOpenEventHandler(object sender, EventArgs e)
        {
            SetLastSuccessfullSocketCommunicationTime();
			mLog.LogMessage(typeof(DoshiiWebSocketsCommunication), Enums.DoshiiLogLevels.Debug, string.Format("Doshii: WebScokets connection successfully open to {0}", m_WebSocketsConnection.Url.ToString()));
            StartHeartbeatThread();
            SocketCommunicationEstablishedEvent(this, e);
        }

        #endregion

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Sets the last time there was a successful sockets connection. 
        /// </summary>
        internal virtual void SetLastSuccessfullSocketCommunicationTime()
        {
            m_LastSuccessfullSocketMessageTime = DateTime.Now;
        }

        /// <summary>
        /// DO NOT USE, All fields, properties, methods in this class are for internal use and should not be used by the POS.
        /// Tests the time out value for the socket connection has not been reached.
        /// </summary>
        /// <returns></returns>
        internal virtual bool TestTimeOutValue()
        {
            if (m_LastSuccessfullSocketMessageTime.AddSeconds((double)m_SocketConnectionTimeOutValue) < DateTime.Now)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

		#region IDisposable Members

		/// <summary>
		/// Cleanly disposes of the instance and its member variables.
		/// </summary>
		public void Dispose()
		{
			mLog = null;
			m_DoshiiLogic = null;
			m_WebSocketsConnection = null;
		}

		#endregion
	}
}
