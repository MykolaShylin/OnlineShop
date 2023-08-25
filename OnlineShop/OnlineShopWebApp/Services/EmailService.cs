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
                orderDetail += $"<h4>Название: {@EnumHelper.GetDisplayName(item.Product.Brand)} - {item.Product.Name} - {item.Product.Flavor.Name}</h4>\r\n" +                                
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

            await SendMessage(emailMessage);
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

            await SendMessage(emailMessage);
        }

        public async Task SendRegistrationConfirmMessageAsync(string email, string password)
        {
            using var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Bull Body, Администрация сайта", "bullbody.ua@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = "Успешная регистрация";


            emailMessage.Body = new BodyBuilder()
            {
                HtmlBody = password == null ? $"<h3>Поздравляем, вы успешно зарегестрировались на нашем сайте</h3>" :
                $"<h3>Поздравляем, вы успешно зарегестрировались на нашем сайте</h3><br/>" +
                $"<h3>Ваш временный пароль - <span class='h2' style='color:red'>{password}</span> - пожалуйста, смените его в своем личном кабинете</h3>"
            }
            .ToMessageBody();

            await SendMessage(emailMessage);
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

            await SendMessage(emailMessage);
        }

        public async Task SendMessage(MimeMessage message)
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 465, true);
                await client.AuthenticateAsync("bullbody.ua@gmail.com", "wwduretiamqmywuf");
                await client.SendAsync(message);

                await client.DisconnectAsync(true);
            }
        }
    }
}
