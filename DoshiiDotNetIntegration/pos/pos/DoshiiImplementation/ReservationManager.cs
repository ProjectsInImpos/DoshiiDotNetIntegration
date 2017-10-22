using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Interfaces;
using DoshiiDotNetIntegration.Models;

namespace pos.DoshiiImplementation
{
    public class ReservationManager : IReservationManager
    {
        public void CreateBookingOnPos(Booking booking)
        {
            LiveData.BookingList.Add(booking);
        }

        public void UpdateBookingOnPos(Booking booking)
        {
            foreach (var book in LiveData.BookingList.Where(b => b.Id == booking.Id))
            {
                book.App = booking.App;
                book.CheckinId = booking.CheckinId;
                book.Consumer = booking.Consumer;
                book.Covers = booking.Covers;
                book.Date = booking.Date;
                book.TableNames = booking.TableNames;
            }
        }

        public void DeleteBookingOnPos(Booking booking)
        {
            LiveData.BookingList.RemoveAll(b => b.Id == booking.Id);
        }

        public void RecordCheckinForBooking(string bookingId, string checkinId)
        {
            foreach (var book in LiveData.BookingList.Where(b => b.Id == bookingId))
            {
                book.CheckinId = checkinId;
            }
        }

        public IEnumerable<Booking> GetBookingsFromPos()
        {
            return LiveData.BookingList;
        }

        public void RecordCheckinForBooking(Booking booking, Checkin checkin)
        {
            var bookingId = booking.Id;
            foreach (var book in LiveData.BookingList.Where(b => b.Id == bookingId))
            {
                book.CheckinId = checkin.Id;
            }

            var checkinToUpdate = LiveData.ChecinList.FirstOrDefault(x => x.Id == checkin.Id);

            if (checkinToUpdate != null)
            {
                foreach (var check in LiveData.ChecinList.Where(x => x.Id == checkin.Id))
                {
                    check.BookingId = checkin.BookingId;
                    check.CompletedAt = checkin.CompletedAt;
                    check.Consumer = checkin.Consumer;
                    check.Covers = checkin.Covers;
                    check.Consumer = checkin.Consumer;
                    check.Ref = checkin.Ref;
                    check.TableNames = checkin.TableNames;
                }
            }
            
        }

        
    }
}
