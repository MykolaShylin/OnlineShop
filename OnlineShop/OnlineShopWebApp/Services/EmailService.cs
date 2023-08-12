using MailKit.Net.Smtp;
using MimeKit;
using OnlineShop.DB.Models;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Services
{
    public class EmailService
    {
        public async Task SendOrderConfirmEmailAsync(string email, List<BasketItemViewModel> items)
        {
            using var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Bull Body, Администрация сайта", "bullbody.ua@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = "Заказ на сайте Bull Body";

            var orderDetail = string.Empty;
            foreach (var item in items)
            {
                orderDetail += $"<h4>Название: {@EnumHelper.GetDisplayName(item.Product.Brand)} - {item.Product.Name} - {item.Product.Flavors.First(x => x.Id == item.ProductInfo.FlavorId).Name}</h4>\r\n" +                                
                                $"<h4>Количество: {item.Amount}</h4>\r\n" +
                                $"<h4>Скидка: {item.ProductInfo.DiscountPercent}%</h4>\r\n" +
                                $"<h4>Цена: {item.Product.DiscountCost}</h4>\r\n" +
                                $"<h4>Цена за количество: {item.TotalAmount}</h4>\r\n";

            }

            orderDetail += $"<h2 style=\"color: red;\">Сумма заказа: {items.Sum(x => x.TotalAmount)}</h2>\r\n";

            emailMessage.Body = new BodyBuilder()
            {
                HtmlBody = "<h3 style=\"color: green;\">Ваш заказ успешно оформлен, ожидайте звонка для подтверждения</h3>" + orderDetail
            }
            .ToMessageBody();


            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 465, true);
                await client.AuthenticateAsync("bullbody.ua@gmail.com", "wwduretiamqmywuf");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }

        public async Task SendEmailConfirmAsync(string email, string callbackUrl)
        {
            using var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Bull Body, Администрация сайта", "bullbody.ua@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = "Подтверждение почты";


            emailMessage.Body = new BodyBuilder()
            {
                HtmlBody = $"<h2>Подтвердите регистрацию, <a href='{callbackUrl}'>перейдя по ссылке:</a></h2>"
            }
            .ToMessageBody();


            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 465, true);
                await client.AuthenticateAsync("bullbody.ua@gmail.com", "wwduretiamqmywuf");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }

        public async Task SendEmailResetPasswordAsync(string email, string callbackUrl)
        {
            using var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Bull Body, Администрация сайта", "bullbody.ua@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = "Сброс пароля";


            emailMessage.Body = new BodyBuilder()
            {
                HtmlBody = $"<h2>Для сброса пароля, <a href='{callbackUrl}'>перейдите по ссылке:</a></h2>"
            }
            .ToMessageBody();


            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 465, true);
                await client.AuthenticateAsync("bullbody.ua@gmail.com", "wwduretiamqmywuf");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
