using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.CommunicationLogic;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Exceptions;
using DoshiiDotNetIntegration.Helpers;
using DoshiiDotNetIntegration.Interfaces;
using DoshiiDotNetIntegration.Models;

namespace DoshiiDotNetIntegration.Controllers
{
    /// <summary>
    /// This class is used internally by the SDK to manage the SDK to manage the business logic handling reservations.
    /// </summary>
    internal class ReservationController
    {
        /// <summary>
        /// prop for the local <see cref="ControllersCollection"/> instance. 
        /// </summary>
        internal Models.ControllersCollection _controllersCollection;

        /// <summary>
        /// prop for the local <see cref="HttpController"/> instance.
        /// </summary>
        internal HttpController _httpComs;

        /// <summary>
        /// constructor. 
        /// </summary>
        /// <param name="controllerCollection"></param>
        /// <param name="httpComs"></param>
        internal ReservationController(Models.ControllersCollection controllerCollection, HttpController httpComs)
        {
            if (controllerCollection == null)
            {
                throw new NullReferenceException("controller cannot be null");
            }
            _controllersCollection = controllerCollection;
            if (_controllersCollection.LoggingController == null)
            {
                throw new NullReferenceException("doshiiLogger cannot be null");
            }

            if (_controllersCollection.ReservationManager == null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(OrderingController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - reservationManager cannot be null");
                throw new NullReferenceException("rewardManager cannot be null");
            }
            if (_controllersCollection.OrderingManager == null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(OrderingController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - orderingManager cannot be null");
                throw new NullReferenceException("orderingManager cannot be null");
            }
            if (_controllersCollection.OrderingController == null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(OrderingController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - orderingController cannot be null");
                throw new NullReferenceException("orderingController cannot be null");
            }
            if (httpComs == null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(TransactionController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - httpComs cannot be null");
                throw new NullReferenceException("httpComs cannot be null");
            }
            _httpComs = httpComs;
        }

        /// <summary>
        /// gets a booking for the venue from Doshii based on the provided bookingId. 
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        internal virtual Booking GetBooking(String bookingId)
        {
            try
            {
                return _httpComs.GetBooking(bookingId);
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// get all the bookings from Doshii for a venue within the provided dateTime range. 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        internal virtual List<Booking> GetBookingsFrom1hrAgoToMaxDate()
        {
            try
            {
                return _httpComs.GetBookings(DateTime.Now.AddHours(-1), DateTime.MaxValue).ToList();
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }
        
        /// <summary>
        /// get all the bookings from Doshii for a venue within the provided dateTime range. 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        internal virtual List<Booking> GetBookings(DateTime from, DateTime to)
        {
            try
            {
                return _httpComs.GetBookings(from, to).ToList();
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// attempts to seat a booking from Doshii
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="checkin"></param>
        /// <param name="posOrderId"></param>
        /// <returns></returns>
        internal ActionResultBasic SeatBooking(String bookingId, Checkin checkin, String posOrderId = null)
        {
            _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: pos Seating Booking '{0}'", bookingId));

            Order order = null;
            if (posOrderId != null)
            {
                try
                {
                    order = _controllersCollection.OrderingManager.RetrieveOrder(posOrderId);
                    order.Version = _controllersCollection.OrderingManager.RetrieveOrderVersion(posOrderId);
                    order.CheckinId = _controllersCollection.OrderingManager.RetrieveCheckinIdForOrder(posOrderId);
                    order.Status = "accepted";
                }
                catch (OrderDoesNotExistOnPosException dne)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Warning, "Doshii: Order does not exist on POS during seating");
                    throw dne;
                }

                if (order == null)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Warning, "Doshii: NULL Order returned from POS during seating");
                    throw new OrderDoesNotExistOnPosException("Doshii: The pos returned a null Order during seating", new OrderUpdateException());
                }

                if (!String.IsNullOrEmpty(order.CheckinId))
                {
                    try
                    {
                        Checkin orderCheckin = _httpComs.GetCheckin(order.CheckinId);
                        if (orderCheckin != null)
                        {
                            if (orderCheckin.Id.CompareTo(checkin.Id) != 0)
                            {
                                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, "Doshii: Order checkin id not equal to booking checkin id");
                                throw new BookingCheckinException("Doshii: Order checkin id != booking checkin id");
                            }
                            if (orderCheckin.Covers != checkin.Covers)
                            {
                                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, "Doshii: Order covers not equal to booking covers");
                                throw new BookingCheckinException("Doshii: Order covers != booking covers");
                            }
                            if (orderCheckin.Consumer.CompareTo(checkin.Consumer) != 0)
                            {
                                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, "Doshii: Order consumer not equal to booking consumer");
                                throw new BookingCheckinException("Doshii: Order consumer != booking consumer");
                            }
                            if (orderCheckin.TableNames.All(o => checkin.TableNames.Contains(o)) && orderCheckin.TableNames.Count == checkin.TableNames.Count)
                            {
                                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, "Doshii: Order Tables not equal to booking tables");
                                throw new BookingCheckinException("Doshii: Order tables != booking tables");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Fatal, "Doshii: an exception was thrown getting the checking", ex);
                        throw ex;
                    }
                }
            }

            Checkin bookingCheckin = null;
            try
            {
                bookingCheckin = _httpComs.SeatBooking(bookingId, checkin);
                if (bookingCheckin == null)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: There was an error generating a new checkin through Doshii, the seat booking could not be completed."));
                    return false;
                }
            }
            catch (Exception ex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: a exception was thrown while attempting a seat booking Id{0} : {1}", bookingId, ex));
                throw new BookingUpdateException(string.Format("Doshii: a exception was thrown during an attempt to seat a booking. Id{0}", bookingId), ex);
            }

            _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format("Doshii: Booking Seated."));

            _controllersCollection.ReservationManager.RecordCheckinForBooking(bookingId, bookingCheckin.Id);

            if (order != null)
            {
                order.CheckinId = bookingCheckin.Id;
                Order returnedOrder = _controllersCollection.OrderingController.UpdateOrder(order);
                if (returnedOrder == null)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// syncs the pos members with the Doshii members, 
        /// NOTE: members that exist on the pos but do not exist on doshii will be deleted from the pos with a call to <see cref="IRewardManager.DeleteMemberOnPos"/>
        /// </summary>
        /// <returns></returns>
        internal virtual bool SyncDoshiiBookingsWithPosBookings()
        {
            try
            {
                List<Booking> DoshiiBookingList = GetBookingsFrom1hrAgoToMaxDate().ToList();
                List<Booking> PosBookingList = _controllersCollection.ReservationManager.GetBookingsFromPos().ToList();

                var doshiiBookingHashSet = new HashSet<string>(DoshiiBookingList.Select(p => p.Id));
                var posBookingHashSet = new HashSet<string>(PosBookingList.Select(p => p.Id));

                var bookingsNotInDoshii = PosBookingList.Where(p => !doshiiBookingHashSet.Contains(p.Id));
                foreach (var book in bookingsNotInDoshii)
                {
                    try
                    {
                        _controllersCollection.ReservationManager.DeleteBookingOnPos(book);
                    }
                    catch (Exception ex)
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: There was an exception deleting a booking on the pos with doshii BookingId {0}", book.Id), ex);
                    }

                }

                var bookingsInPos = DoshiiBookingList.Where(p => posBookingHashSet.Contains(p.Id));
                foreach (var book in bookingsInPos)
                {
                    Booking posBooking = PosBookingList.FirstOrDefault(p => p.Id == book.Id);
                    if (!book.Equals(posBooking))
                    {
                        try
                        {
                            _controllersCollection.ReservationManager.UpdateBookingOnPos(book);
                        }
                        catch (Exception ex)
                        {
                            _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: There was an exception updating a booking on the pos with doshii bookingId {0}", book.Id), ex);
                        }

                    }
                }

                var bookingsNotInPos = DoshiiBookingList.Where(p => !posBookingHashSet.Contains(p.Id));
                foreach (var book in bookingsNotInPos)
                {
                    try
                    {
                        _controllersCollection.ReservationManager.CreateBookingOnPos(book);
                    }
                    catch (Exception ex)
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: There was an exception creating a booking on the pos with doshii bookingId {0}", book.Id), ex);
                    }

                }


                return true;
            }
            catch (Exception ex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format("Doshii: There was an exception while attempting to sync Doshii bookings with the pos"), ex);
                return false;
            }
        }
    }
}
