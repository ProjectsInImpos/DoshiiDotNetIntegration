using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DoshiiDotNetIntegration.CommunicationLogic;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Exceptions;
using DoshiiDotNetIntegration.Helpers;
using DoshiiDotNetIntegration.Interfaces;
using DoshiiDotNetIntegration.Models;
using DoshiiDotNetIntegration.Models.ActionResults;
using DoshiiDotNetIntegration.Models.Json;

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
                _controllersCollection.LoggingController.LogMessage(typeof(OrderingController), DoshiiLogLevels.Fatal, " Initialization failed - reservationManager cannot be null");
                throw new NullReferenceException("reservationManager cannot be null");
            }
            if (_controllersCollection.OrderingManager == null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(OrderingController), DoshiiLogLevels.Fatal, " Initialization failed - orderingManager cannot be null");
                throw new NullReferenceException("orderingManager cannot be null");
            }
            if (_controllersCollection.OrderingController == null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(OrderingController), DoshiiLogLevels.Fatal, " Initialization failed - orderingController cannot be null");
                throw new NullReferenceException("orderingController cannot be null");
            }
            if (httpComs == null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(TransactionController), DoshiiLogLevels.Fatal, " Initialization failed - httpComs cannot be null");
                throw new NullReferenceException("httpComs cannot be null");
            }
            _httpComs = httpComs;
        }

        /// <summary>
        /// gets a booking for the venue from Doshii based on the provided bookingId. 
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="fast">Default value is true . This parameter fill the detial checkinobject if checkin is available </param>
        /// <returns></returns>
        internal virtual ObjectActionResult<Booking> GetBooking(String bookingId, bool fast=true)
        {
            try
            {
                return _httpComs.GetBooking(bookingId,fast);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }


        internal virtual ObjectActionResult<Checkin> GetCheckin(string checkinId)
        {
            try
            {
                return _httpComs.GetCheckin(checkinId);
            }
            catch (Exception rex)
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
        internal virtual ObjectActionResult<List<Booking>> GetBookingsFrom1hrAgoToMaxDate()
        {
            try
            {
                return GetBookings(DateTime.Now.AddHours(-1), DateTime.MaxValue);
            }
            catch (Exception rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// get all the bookings from Doshii for a venue within the dateTime range from 1 hr ago to 1 month ahead. 
        /// </summary>
        /// <returns></returns>
        internal virtual IEnumerable<Booking> GetBookingsFrom1hrAgoTo1MonthAhead()
        {
            var returnResult = new List<Booking>();
            var offset = 0;
            var limit = 50;
            try
            {
                
                var from = DateTime.Now.AddHours(-1); 
                var to = DateTime.Now.AddMonths(1);

                

                var result= _httpComs.GetBookings(from, to, query: new {offset, limit});
                while (result.ReturnObject.Count > 0)
                {
                    offset += limit;
                    returnResult.AddRange(result.ReturnObject);
                    result = _httpComs.GetBookings(from, to, query: new {offset, limit});
                }

            }
            catch (Exception rex)
            {
                throw rex;
            }
            return returnResult;
        }

        /// <summary>
        /// get all the bookings from Doshii for a venue within the provided dateTime range. 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        internal virtual ObjectActionResult<List<Booking>> GetBookings(DateTime from, DateTime to)
        {
            try
            {
                return _httpComs.GetBookings(from, to);
            }
            catch (Exception ex)
            {
                throw ex;
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
            _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format(" pos Seating Booking '{0}'", bookingId));

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
                    _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Warning, " Order does not exist on POS during seating");
                    throw dne;
                }

                if (!String.IsNullOrEmpty(order.CheckinId))
                {
                    try
                    {
                        var orderCheckinResult = _httpComs.GetCheckin(order.CheckinId);
                        if (orderCheckinResult.ReturnObject != null)
                        {
                            if (orderCheckinResult.ReturnObject.Id.CompareTo(checkin.Id) != 0)
                            {
                                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, " Order checkin id not equal to booking checkin id");
                                throw new BookingCheckinException(" Order checkin id != booking checkin id");
                            }
                            if (orderCheckinResult.ReturnObject.Covers != checkin.Covers)
                            {
                                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, " Order covers not equal to booking covers");
                                throw new BookingCheckinException(" Order covers != booking covers");
                            }
                            if (orderCheckinResult.ReturnObject.Consumer.CompareTo(checkin.Consumer) != 0)
                            {
                                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, " Order consumer not equal to booking consumer");
                                throw new BookingCheckinException(" Order consumer != booking consumer");
                            }
                            if (orderCheckinResult.ReturnObject.TableNames.All(o => checkin.TableNames.Contains(o)) && orderCheckinResult.ReturnObject.TableNames.Count == checkin.TableNames.Count)
                            {
                                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, " Order Tables not equal to booking tables");
                                throw new BookingCheckinException(" Order tables != booking tables");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Fatal, " an exception was thrown getting the checking", ex);
                        throw ex;
                    }
                }
            }

            ObjectActionResult<Checkin> bookingCheckinResult = null;
            try
            {
                bookingCheckinResult = _httpComs.SeatBooking(bookingId, checkin);
                if (!bookingCheckinResult.Success)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" There was an error generating a new checkin through Doshii, the seat booking could not be completed."));
                    return new ActionResultBasic()
                    {
                        Success = false,
                        FailReason = bookingCheckinResult.FailReason
                    };
                }
            }
            catch (Exception ex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" a exception was thrown while attempting a seat booking Id{0} : {1}", bookingId, ex));
                throw new BookingUpdateException(string.Format(" a exception was thrown during an attempt to seat a booking. Id{0}", bookingId), ex);
            }

            _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format(" Booking Seated."));

            _controllersCollection.ReservationManager.RecordCheckinForBooking(bookingId, bookingCheckinResult.ReturnObject);

            if (order != null)
            {
                order.CheckinId = bookingCheckinResult.ReturnObject.Id;
                var returnedOrder = _controllersCollection.OrderingController.UpdateOrder(order);
                if (returnedOrder.ReturnObject == null)
                {
                    return new ActionResultBasic()
                    {
                        Success = false,
                        FailReason = returnedOrder.FailReason
                    };
                }
                    
            }

            return new ActionResultBasic()
            {
                Success = true
            };
        }


        internal ObjectActionResult<Checkin> SeatBookingWithoutCheckin(String bookingId, String posOrderId = null)
        {
            _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format(" pos Seating Booking '{0}'", bookingId));

            Order order = null;
            if (posOrderId != null)
            {
                try
                {
                    order = _controllersCollection.OrderingManager.RetrieveOrder(posOrderId);
                    order.Version = _controllersCollection.OrderingManager.RetrieveOrderVersion(posOrderId);
                    order.CheckinId = _controllersCollection.OrderingManager.RetrieveCheckinIdForOrder(posOrderId);
                    order.Status = "accepted";
                    var result = _controllersCollection.OrderingController.UpdateOrder(order);
                    if (!result.Success)
                    {
                        return new ObjectActionResult<Checkin>()
                        {
                            Success = false,
                            FailReason = result.FailReason
                        };
                    }
                }
                catch (OrderDoesNotExistOnPosException dne)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController),
                        DoshiiLogLevels.Warning, " Order does not exist on POS during seating");
                    throw dne;
                }
            }

            ObjectActionResult<Checkin> bookingCheckinResult = null;
            try
            {
                bookingCheckinResult = _httpComs.SeatBookingWithoutCheckin(bookingId);
                if (!bookingCheckinResult.Success)
                {
                    _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" There was an error generating a new checkin through Doshii, the seat booking could not be completed."));
                    return new ObjectActionResult<Checkin>()
                    {
                        Success = false,
                        FailReason = bookingCheckinResult.FailReason
                    };
                }
            }
            catch (Exception ex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" a exception was thrown while attempting a seat booking Id{0} : {1}", bookingId, ex));
                throw new BookingUpdateException(string.Format(" a exception was thrown during an attempt to seat a booking. Id{0}", bookingId), ex);
            }

            _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format(" Booking Seated."));

            _controllersCollection.ReservationManager.RecordCheckinForBooking(bookingId, bookingCheckinResult.ReturnObject);

            if (order != null)
            {
                order.CheckinId = bookingCheckinResult.ReturnObject.Id;
                var returnedOrder = _controllersCollection.OrderingController.UpdateOrder(order);
                if (!returnedOrder.Success)
                {
                    return new ObjectActionResult<Checkin>()
                    {
                        Success = false,
                        FailReason = returnedOrder.FailReason
                    };
                }
            }

            return bookingCheckinResult;
        }


        /// <summary>
        /// syncs the pos members with the Doshii members, 
        /// NOTE: members that exist on the pos but do not exist on doshii will be deleted from the pos with a call to <see cref="IRewardManager.DeleteMemberOnPos"/>
        /// </summary>
        /// <returns></returns>
        internal virtual ActionResultBasic SyncDoshiiBookingsWithPosBookings()
        {
            try
            {
                StringBuilder failedReasonBuilder = new StringBuilder();
                List<Booking> DoshiiBookingList = GetBookingsFrom1hrAgoTo1MonthAhead().ToList();
                List<Booking> PosBookingList = _controllersCollection.ReservationManager.GetBookingsFromPos().ToList();

                var doshiiBookingHashSet = new HashSet<string>(DoshiiBookingList.Select(p => p.Id));
                var posBookingHashSet = new HashSet<string>(PosBookingList.Select(p => p.Id));

                var bookingsNotInDoshii = PosBookingList.Where(p => !doshiiBookingHashSet.Contains(p.Id) || string.IsNullOrEmpty(p.Id) || p.Id == null);
                foreach (var book in bookingsNotInDoshii)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(book.Id) || book.Id == null)
                        {
                            var createResult = _controllersCollection.ReservationController.UpdateBooking(book);
                            if (createResult.Success)
                            {
                                _controllersCollection.ReservationManager.UpdateBookingOnPos(createResult.ReturnObject);
                            }
                        }
                        else
                        {
                            _controllersCollection.ReservationManager.DeleteBookingOnPos(book);
                        }
                    }
                    catch (Exception ex)
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" There was an exception deleting a booking on the pos with doshii BookingId {0}", book.Id), ex);
                        failedReasonBuilder.AppendLine(string.Format("booking with Id {0} failed to delete from the pos",
                            book.Id));
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
                            _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" There was an exception updating a booking on the pos with doshii bookingId {0}", book.Id), ex);
                            failedReasonBuilder.AppendLine(string.Format("booking with Id {0} failed to update on the pos",
                            book.Id));
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
                        _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" There was an exception creating a booking on the pos with doshii bookingId {0}", book.Id), ex);
                        failedReasonBuilder.AppendLine(string.Format("booking with Id {0} failed to create on the pos",
                            book.Id));
                    }

                }

                if (string.IsNullOrEmpty(failedReasonBuilder.ToString()))
                {
                    return new ActionResultBasic()
                    {
                        Success = true
                    };
                }
                else
                {
                    return new ActionResultBasic()
                    {
                        Success = false,
                        FailReason = failedReasonBuilder.ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" There was an exception while attempting to sync Doshii bookings with the pos"), ex);
                return new ActionResultBasic()
                {
                    Success = false,
                    FailReason = DoshiiStrings.GetThereWasAnExceptionSeeLogForDetails("syncing bookings")
                };
            }
        }

        internal virtual ObjectActionResult<Booking> UpdateBooking(Booking booking)
        {
            var jsonBooking = Mapper.Map<JsonBooking>(booking);
            _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format(" pos updating Booking - '{0}'", jsonBooking.ToJsonString()));

            try
            {
                if (string.IsNullOrEmpty(booking.Id))
                {
                    return _httpComs.PostBooking(booking);
                }
                else
                {
                    return _httpComs.PutBooking(booking);
                }
            }
            catch (Exception ex)
            {
                throw new BookingUpdateException("Update booking not successful", ex);
            }
        }

        internal virtual ActionResultBasic DeleteBooking(string bookingId)
        {
            _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Debug, string.Format(" pos deleting Booking id- '{0}'", bookingId));
            try
            {
                return _httpComs.DeleteBooking(bookingId);
            }
            catch (Exception ex)
            {
                throw new BookingUpdateException("delete booking not successful", ex);
            }
        }
    }
}
