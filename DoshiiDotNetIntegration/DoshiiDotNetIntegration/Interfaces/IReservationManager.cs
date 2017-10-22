using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models;

namespace DoshiiDotNetIntegration.Interfaces
{
	   /// <summary>
    /// Implementations of this interface is required to handle reservation functionality in Doshii.
    /// <para/>The POS should implement this interface to enable reservations and table bookings.
    /// </summary>
    /// <remarks>
    /// <para/><see cref="DoshiiController"/> uses this interface as a callback mechanism 
    /// to the POS for reservation functions. 
    /// <para>
    /// </para>
    /// </remarks>
	public interface IReservationManager
	{
		/// <summary>
		/// This method should create a doshii booking on the Pos.
		/// </summary>
		/// <param name="booking"></param>
		/// <returns></returns>
		void CreateBookingOnPos(Booking booking);

		/// <summary>
		/// This method should update a doshii booking on the Pos.
        /// This will be called whenever a doshii booking is updated in the cloud. 
        /// The method will also be called when the sdk is initialized it does a sync with the cloud it will not have an Id when it is updated in the cloud so another prop will need to be used to identify it on the pos. 
        /// </summary>
		/// <param name="booking"></param>
		/// <returns></returns>
		void UpdateBookingOnPos(Booking booking);

		/// <summary>
		/// This method should delete a doshii booking on the Pos.
		/// </summary>
		/// <param name="booking"></param>
		/// <returns></returns>
		void DeleteBookingOnPos(Booking booking);

        /// <summary>
        /// The <see cref="DoshiiController"/> uses this call to inform the pos the checkin 
        /// associated with an booking stored on Doshii. The <paramref name="checkinId"/> string must be persisted in
        /// the POS against the booking - the CheckinId is the link between booking and orders and tables and also consumers, in the doshii API. 
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="checkinId">The current CheckinId related to the booking in Doshii.</param>
        /// <exception cref="DoshiiDotNetIntegration.Exceptions.BookingDoesNotExistOnPosException">This exception 
        /// should be thrown when there is no booking in the POS with the corresponding 
        /// <paramref name="bookingId"/>.</exception>
        void RecordCheckinForBooking(Booking booking, Checkin checkin);

        /// <summary>
        /// This method should retreive all the BookingsWithDateFilter from the pos. 
        /// </summary>
        /// <param name="DoshiiMemberId"></param>
        /// <returns></returns>
        IEnumerable<DoshiiDotNetIntegration.Models.Booking> GetBookingsFromPos();
    }
}
