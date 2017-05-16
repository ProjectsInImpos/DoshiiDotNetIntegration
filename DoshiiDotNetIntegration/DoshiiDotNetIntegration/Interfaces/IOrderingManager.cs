using DoshiiDotNetIntegration.Models;
using System.Collections.Generic;

namespace DoshiiDotNetIntegration.Interfaces
{
	/// <summary>
	/// Implementations of this interface is required to handle orders in Doshii.
	/// <para/>The POS should implement this interface to accept new orders and updates to orders from Doshii.
    /// <para/>Version control on orders is also managed through the POS implementation of this interface.
	/// </summary>
	/// <remarks>
	/// This interface is a core Doshii interface required for implementation on the POS side. 
    /// <para/><see cref="DoshiiController"/> uses this interface as a callback mechanism 
	/// to the POS for basic Order functions. 
    /// <para/>It should be noted however that this interface is not the handler for extension modules such as Order@Table which will be implemented in a separate
	/// callback interface.
	/// <para>
	/// When a partner wishes to perform an action on an Order in Doshii, the SDK emits a call to 
	/// <see cref="DoshiiDotNetIntegration.Interfaces.IOrderingManager.RetrieveOrder(string)"/> to
	/// retrieve the current state of the Order in the POS. 
    /// <para/>After a partner has mutated the Order in Doshii
	/// in some way, the SDK emits a call to 
	/// <see cref="DoshiiDotNetIntegration.Interfaces.IOrderingManager.RecordOrderVersion(string, string)"/>
	/// to inform the POS of the update success. 
	/// </para>
	/// </remarks>
	public interface IOrderingManager
	{
		/// <summary>
		/// The <see cref="DoshiiController"/> uses this call to request the current
		/// details of an Order from the POS.
		/// </summary>
		/// <remarks>
		/// POS implementations of this function are required to return the full details of the Order with
        /// the corresponding <paramref name="posOrderId"/>.
		/// </remarks>
		/// <param name="posOrderId">The unique identifier of the Order being queried in the POS.</param>
		/// <returns>The Order details </returns>
		/// <exception cref="DoshiiDotNetIntegration.Exceptions.OrderDoesNotExistOnPosException">This exception 
		/// should be thrown when there is no Order in the POS with the corresponding 
		/// <paramref name="posOrderId"/>.</exception>
		DoshiiDotNetIntegration.Models.Order RetrieveOrder(string posOrderId);

		/// <summary>
		/// The <see cref="DoshiiController"/> uses this call to inform the pos
		/// that an Order has been updated. The <paramref name="version"/> string must be persisted in
		/// the POS and passed back when the POS updates an Order. 
		/// </summary>
		/// <remarks>
		/// The current <paramref name="version"/> is used by Doshii for conflict resolution, but the POS is 
		/// the final arbiter on the state of an Order.
		/// </remarks>
		/// <param name="posOrderId">The unique identifier of the Order being updated in the POS.</param>
		/// <param name="version">The current version of the Order in Doshii.</param>
		/// <exception cref="DoshiiDotNetIntegration.Exceptions.OrderDoesNotExistOnPosException">This exception 
		/// should be thrown when there is no Order in the POS with the corresponding 
		/// <paramref name="posOrderId"/>.</exception>
		void RecordOrderVersion(string posOrderId, string version);

		/// <summary>
		/// The <see cref="DoshiiController"/> uses this call to request the current
		/// version of an Order in the POS.
		/// </summary>
		/// <param name="posOrderId">The unique identifier of the Order being queried on the POS.</param>
		/// <returns>The current version of the Order in the POS.</returns>
		/// <exception cref="DoshiiDotNetIntegration.Exceptions.OrderDoesNotExistOnPosException">This exception 
		/// should be thrown when there is no Order in the POS with the corresponding 
		/// <paramref name="posOrderId"/>.</exception>
		string RetrieveOrderVersion(string posOrderId);

        /// <summary>
        /// The <see cref="DoshiiController"/> uses this call to inform the pos the checkin 
        /// associated with an Order stored on Doshii. The <paramref name="checkinId"/> string must be persisted in
        /// the POS against the Order - the checkinId is the link between orders and tables and also orders and consumers, in the doshii API. 
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="posOrderId">The unique identifier of the Order being updated in the POS.</param>
        /// <param name="checkinId">The current checkinId related to the Order in Doshii.</param>
        /// <exception cref="DoshiiDotNetIntegration.Exceptions.OrderDoesNotExistOnPosException">This exception 
        /// should be thrown when there is no Order in the POS with the corresponding 
        /// <paramref name="posOrderId"/>.</exception>
        void RecordCheckinForOrder(string posOrderId, string checkinId);

        /// <summary>
        /// The <see cref="DoshiiController"/> uses this call to request the current
        /// checkinId associated with an Order.
        /// </summary>
        /// <param name="posOrderId">The unique identifier of the Order being queried on the POS.</param>
        /// <returns>The current version of the Order in the POS.</returns>
        /// <exception cref="DoshiiDotNetIntegration.Exceptions.OrderDoesNotExistOnPosException">This exception 
        /// should be thrown when there is no Order in the POS with the corresponding 
        /// <paramref name="posOrderId"/>.</exception>
        string RetrieveCheckinIdForOrder(string posOrderId);

        /// <summary>
        /// The <see cref="DoshiiController"/> calls this method on the pos so the pos can confirm the acceptance of the Order. 
        /// The pos must check that the Order can be made on the pos, and that the transactions total the correct amount for payment of the Order in full
        /// The pos cannot modify the <see cref="Transaction"/> objects in the transaction list, during this process as the amount has already been confirmed with the consumer.
        /// If the <see cref="Order"/> is accepted the POS must update the <see cref="Order.Id"/> property with the pos reference to the Order and call <see cref="DoshiiController.AcceptOrderAheadCreation"/> with the Order, and the
        /// if the response from <see cref="DoshiiController.AcceptOrderAheadCreation"/> is successful - if the response is false this could indicate that the Order has been canceled or changed on doshii and the pos will receive another create notification if necessary.
        /// The transaction should not be recorded on the POS during this method.
        /// The Pos will receive a call to <see cref="ITransactionManager.RecordSuccessfulPayment"/> to record the transaction
        /// If the <see cref="Order"/> or the <see cref="Transaction"/> is rejected the pos should call <see cref="DoshiiController.RejectOrderAheadCreation"/> with the Order.
        /// </summary>
        /// <param name="order">
        /// The <see cref="Order"/> to be approved
        /// </param>
        /// <param name="transactionList">
        /// A List of <see cref="Transaction"/> to be approved
        /// </param>
        /// /// <param name="consumer">
        /// The <see cref="Consumer"/> associated with the Order
        /// </param>
        /// <returns></returns>
        void ConfirmNewDeliveryOrderWithFullPayment(Order order, Consumer consumer, IEnumerable<Transaction> transactionList);

        /// <summary>
        /// The <see cref="DoshiiController"/> calls this method on the pos so the pos can confirm the acceptance of the Order. 
        /// The pos must check that the Order can be made on the pos.
        /// if the <see cref="Order"/> is accepted the POS must update the <see cref="Order.Id"/> property with the pos reference to the Order and call <see cref="DoshiiController.AcceptOrderAheadCreation"/> with the Order, 
        /// if the response from <see cref="DoshiiController.AcceptOrderAheadCreation"/> is successful - if the response is false this could indicate that the Order has been canceled or changed on doshii and the pos will receive another create notification if necessary.
        /// If the <see cref="Order"/> is rejected the pos should call <see cref="DoshiiController.RejectOrderAheadCreation"/> with the Order.
        /// </summary>
        /// <param name="order">
        /// The <see cref="Order"/> to be approved
        /// </param>
        /// <param name="consumer">
        /// The <see cref="Consumer"/> associated with the Order
        /// <para/> The consumer object contains data including;
        /// <item>The consumer address</item>
        /// <item>The consumer phone number</item>
        /// <item>The consumer name</item>
        /// <item>Special notes relating to the Order</item>
        /// </param>
        /// <returns></returns>
	    void ConfirmNewDeliveryOrder(Order order, Consumer consumer);

        /// <summary>
        /// The <see cref="DoshiiController"/> calls this method on the pos so the pos can confirm the acceptance of the Order. 
        /// The pos must check that the Order can be made on the pos, and that the transactions total the correct amount for payment of the Order in full
        /// The pos cannot modify the <see cref="Transaction"/> objects in the transaction list, during this process as the amount has already been confirmed with the consumer.
        /// If the <see cref="Order"/> is accepted the POS must update the <see cref="Order.Id"/> property with the pos reference to the order and and call <see cref="DoshiiController.AcceptOrderAheadCreation"/> with the Order, and the
        /// Order should be made on the pos if the response from <see cref="DoshiiController.AcceptOrderAheadCreation"/> is successful - if the response is false this could indicate that the Order has been canceled or changed on doshii and the pos will receive another create notification if necessary. 
        /// The transaction should not be recorded on the POS during this method. The
        /// he Pos will receive a call to <see cref="ITransactionManager.RecordSuccessfulPayment"/> to record the transaction
        /// If the <see cref="Order"/> or the <see cref="Transaction"/> is rejected the pos should call <see cref="DoshiiController.RejectOrderAheadCreation"/> with the Order.
        /// </summary>
        /// <param name="order">
        /// The <see cref="Order"/> to be approved
        /// </param>
        /// <param name="transactionList">
        /// A List of <see cref="Transaction"/> to be approved
        /// </param>
        /// <param name="consumer">
        /// The <see cref="Consumer"/> associated with the Order
        /// <para/> The consumer object contains data including;
        /// <item>The consumer address</item>
        /// <item>The consumer phone number</item>
        /// <item>The consumer name</item>
        /// <item>Special notes relating to the Order</item>
        /// </param>
        /// <returns></returns>
        void ConfirmNewPickupOrderWithFullPayment(Order order, Consumer consumer, IEnumerable<Transaction> transactionList);

        /// <summary>
        /// The <see cref="DoshiiController"/> calls this method on the pos so the pos can confirm the acceptance of the Order. 
        /// The pos must check that the Order can be made on the pos, and that the transactions total the correct amount for payment of the Order in full
        /// The pos cannot modify the <see cref="Transaction"/> objects in the transaction list, during this process as the amount has already been confirmed with the consumer.
        /// If the <see cref="Order"/> is accepted the POS must update the <see cref="Order.Id"/> property with the pos reference to the Order and and call <see cref="DoshiiController.AcceptOrderAheadCreation"/> with the Order, and the
        /// Order should be made on the pos if the response from <see cref="DoshiiController.AcceptOrderAheadCreation"/> is successful - if the response is false this could indicate that the Order has been canceled or changed on doshii and the pos will receive another create notification if necessary. 
        /// The transaction should not be recorded on the POS during this method. The
        /// he Pos will receive a call to <see cref="ITransactionManager.RecordSuccessfulPayment"/> to record the transaction
        /// If the <see cref="Order"/> or the <see cref="Transaction"/> is rejected the pos should call <see cref="DoshiiController.RejectOrderAheadCreation"/> with the Order.
        /// </summary>
        /// <param name="order">
        /// The <see cref="Order"/> to be approved
        /// </param>
        /// <param name="transactionList">
        /// A List of <see cref="Transaction"/> to be approved
        /// </param>
        /// <param name="consumer">
        /// The <see cref="Consumer"/> associated with the Order
        /// <para/> The consumer object contains data including;
        /// <item>The consumer address</item>
        /// <item>The consumer phone number</item>
        /// <item>The consumer name</item>
        /// <item>Special notes relating to the Order</item>
        /// </param>
        /// <returns></returns>
        void ConfirmNewDineInOrderWithFullPayment(Order order, Consumer consumer, IEnumerable<Transaction> transactionList);

        /// <summary>
        /// The <see cref="DoshiiController"/> calls this method on the pos so the pos can confirm the acceptance of the Order. 
        /// The pos must check that the Order can be made on the pos.
        /// if the <see cref="Order"/> is accepted the POS must update the <see cref="Order.Id"/> property with the pos reference to the Order and call <see cref="DoshiiController.AcceptOrderAheadCreation"/> with the Order, and the 
        /// Order should be made on the pos if the response from <see cref="DoshiiController.AcceptOrderAheadCreation"/> is successful - if the response is false this could indicate that the Order has been canceled or changed on doshii and the pos will receive another create notification if necessary.. 
        /// If the <see cref="Order"/> is rejected the pos should call <see cref="DoshiiController.RejectOrderAheadCreation"/> with the Order.
        /// </summary>
        /// <param name="order">
        /// the <see cref="Order"/> to be approved
        /// </param>
        /// <param name="consumer">
        /// The <see cref="Consumer"/> associated with the Order
        /// <para/> The consumer object contains data including;
        /// <item>The consumer address</item>
        /// <item>The consumer phone number</item>
        /// <item>The consumer name</item>
        /// <item>Special notes relating to the Order</item>
        /// </param>
        /// <returns></returns>
        void ConfirmNewPickupOrder(Order order, Consumer consumer);

        /// <summary>
        /// The <see cref="DoshiiController"/> calls this method on the pos so the pos can confirm the acceptance of the Order. 
        /// The pos must check that the Order can be made on the pos.
        /// if the <see cref="Order"/> is accepted the POS must update the <see cref="Order.Id"/> property with the pos reference to the Order and call <see cref="DoshiiController.AcceptOrderAheadCreation"/> with the Order, and the 
        /// Order should be made on the pos if the response from <see cref="DoshiiController.AcceptOrderAheadCreation"/> is successful - if the response is false this could indicate that the Order has been canceled or changed on doshii and the pos will receive another create notification if necessary.. 
        /// If the <see cref="Order"/> is rejected the pos should call <see cref="DoshiiController.RejectOrderAheadCreation"/> with the Order.
        /// </summary>
        /// <param name="order">
        /// the <see cref="Order"/> to be approved
        /// </param>
        /// <param name="consumer">
        /// The <see cref="Consumer"/> associated with the Order
        /// <para/> The consumer object contains data including;
        /// <item>The consumer address</item>
        /// <item>The consumer phone number</item>
        /// <item>The consumer name</item>
        /// <item>Special notes relating to the Order</item>
        /// </param>
        /// <returns></returns>
        void ConfirmNewDineInOrder(Order order, Consumer consumer);

        /// <summary>
        /// The <see cref="DoshiiController"/> calls this method on the pos so the pos can confirm the acceptance of the Order. 
        /// The pos must check that the Order can be made on the pos, and that the transactions total the correct amount for payment of the Order in full
        /// The pos cannot modify the <see cref="Transaction"/> objects in the transaction list, during this process as the amount has already been confirmed with the consumer.
        /// If the <see cref="Order"/> is accepted the POS must update the <see cref="Order.Id"/> property with the pos reference to the Order and and call <see cref="DoshiiController.AcceptOrderAheadCreation"/> with the Order, and the
        /// Order should be made on the pos if the response from <see cref="DoshiiController.AcceptOrderAheadCreation"/> is successful - if the response is false this could indicate that the Order has been canceled or changed on doshii and the pos will receive another create notification if necessary. 
        /// The transaction should not be recorded on the POS during this method. The
        /// he Pos will receive a call to <see cref="ITransactionManager.RecordSuccessfulPayment"/> to record the transaction
        /// If the <see cref="Order"/> or the <see cref="Transaction"/> is rejected the pos should call <see cref="DoshiiController.RejectOrderAheadCreation"/> with the Order.
        /// </summary>
        /// <param name="order">
        /// The <see cref="Order"/> to be approved
        /// </param>
        /// <param name="transactionList">
        /// A List of <see cref="Transaction"/> to be approved
        /// </param>
        /// <param name="consumer">
        /// The <see cref="Consumer"/> associated with the Order
        /// <para/> The consumer object contains data including;
        /// <item>The consumer address</item>
        /// <item>The consumer phone number</item>
        /// <item>The consumer name</item>
        /// <item>Special notes relating to the Order</item>
        /// </param>
        /// <returns></returns>
        void ConfirmNewUnknownTypeOrderWithFullPayment(Order order, Consumer consumer, IEnumerable<Transaction> transactionList);

        /// <summary>
        /// The <see cref="DoshiiController"/> calls this method on the pos so the pos can confirm the acceptance of the Order. 
        /// The pos must check that the Order can be made on the pos.
        /// if the <see cref="Order"/> is accepted the POS must update the <see cref="Order.Id"/> property with the pos reference to the Order and call <see cref="DoshiiController.AcceptOrderAheadCreation"/> with the Order, and the 
        /// Order should be made on the pos if the response from <see cref="DoshiiController.AcceptOrderAheadCreation"/> is successful - if the response is false this could indicate that the Order has been canceled or changed on doshii and the pos will receive another create notification if necessary.. 
        /// If the <see cref="Order"/> is rejected the pos should call <see cref="DoshiiController.RejectOrderAheadCreation"/> with the Order.
        /// </summary>
        /// <param name="order">
        /// the <see cref="Order"/> to be approved
        /// </param>
        /// <param name="consumer">
        /// The <see cref="Consumer"/> associated with the Order
        /// <para/> The consumer object contains data including;
        /// <item>The consumer address</item>
        /// <item>The consumer phone number</item>
        /// <item>The consumer name</item>
        /// <item>Special notes relating to the Order</item>
        /// </param>
        /// <returns></returns>
        void ConfirmNewUnknownTypeOrder(Order order, Consumer consumer);



        /// <summary>
        /// The <see cref="DoshiiController"/> calls this method on the pos so the pos can process that an Order has a status of venue_cancelled. The venue_cancelled status will be sent to the pos if the Order was canceled by a partner device controlled by the venue to accept, reject, and cancel orders in venue - This is different from a partner cancelling an Order, this will be handled by the other methods. 
        /// The pos must check the status of the Order.
        /// if the <see cref="Order"/> is accepted the POS must update the <see cref="Order.Id"/> property with the pos reference to the Order and call <see cref="DoshiiController.AcceptOrderAheadCreation"/> with the Order, and the 
        /// Order should be made on the pos if the response from <see cref="DoshiiController.AcceptOrderAheadCreation"/> is successful - if the response is false this could indicate that the Order has been canceled or changed on doshii and the pos will receive another create notification if necessary.. 
        /// If the <see cref="Order"/> is rejected the pos should call <see cref="DoshiiController.RejectOrderAheadCreation"/> with the Order.
        /// </summary>
        /// <param name="order">
        /// the <see cref="Order"/> to be approved
        /// </param>
        /// <param name="consumer">
        /// The <see cref="Consumer"/> associated with the Order
        /// <para/> The consumer object contains data including;
        /// <item>The consumer address</item>
        /// <item>The consumer phone number</item>
        /// <item>The consumer name</item>
        /// <item>Special notes relating to the Order</item>
        /// </param>
        /// <returns></returns>
        void ProcessVenueCanceledOrderUpdate(Order order);
	}
}
