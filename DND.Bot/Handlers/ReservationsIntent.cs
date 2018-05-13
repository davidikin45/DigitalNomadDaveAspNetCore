using DND.Bot.Models.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace DND.Bot.Handlers
{
    public class ReservationsIntent
    {
        internal static CommonModel Process(CommonModel commonModel)
        {
            var time = commonModel.Request.Parameters.FirstOrDefault(p => p.Key == "time");
            var timeparse = time.Value;
            if (timeparse.Length == 2)
            {
                timeparse = timeparse + ":00";
            }

            var date = commonModel.Request.Parameters.FirstOrDefault(p => p.Key == "date");
            var number = commonModel.Request.Parameters.FirstOrDefault(p => p.Key == "number");

            var code = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();

            short.TryParse(number.Value, out short guests);
            DateTimeOffset.TryParse(timeparse, out DateTimeOffset timeOnly);
            DateTimeOffset.TryParse(date.Value, out DateTimeOffset dateOnly);

            //using (var data = new TexMexTacosBot.Data.TexMexTacosDataEntities())
            //{
            //    data.Reservations.Add(new Data.Reservation
            //    {
            //        Code = code,
            //        Guests = guests,
            //        Date = date.Value,
            //        Time = time.Value,
            //        DateRequested = DateTime.UtcNow
            //    });

            //    data.SaveChanges();
            //}

            var guestsFormat = guests.ToString();
            var timeFormat = timeOnly.ToString("HH:mm");
            var dateFormat = dateOnly.ToString("yyyy-MM-dd");

            commonModel.Response.Ssml = $"Perfect, your table for {guestsFormat} is reserved for {timeFormat} on {dateFormat}. " +
                $"When you arrive, give them the confirmation code, <say-as interpret-as=\"spell-out\">{code}111</say-as>. " +
                $"Buen provecho!";

            commonModel.Response.Text = $"Perfect, your table for {guestsFormat} is reserved for {timeFormat} on {dateFormat}. " +
                $"When you arrive, give them the confirmation code, {code}. Buen provecho!";

            commonModel.Response.Card = new Card
            {
                Title = "Digital Nomad Dave Tacos Reservations",
                Text = $"Your table for {guestsFormat} is reserved for {timeFormat} on {dateFormat}. Your confirmation code is {code}. Buen provecho!"
            };

            commonModel.Session.EndSession = true;

            return commonModel;
        }
    }
}