using OpenQA.Selenium;
using System.Reflection.Metadata;
using OnlineShop.DB;
using OnlineShop.DB.Models;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;

namespace OnlineShop.UITests
{
    public class LoginTest : IDisposable
    {
        private readonly string url = "https://localhost:5001/UserRegistration/Register";
        private readonly IWebDriver _webDriver;
            
        public LoginTest()
        {
            _webDriver = new EdgeDriver();
        }
        public void Dispose()
        {
            _webDriver.Quit();
            _webDriver.Dispose();
        }

        [Fact]
        public void Register_WhenWxecuted_ReturnsCreateView()
        {
            _webDriver.Navigate().GoToUrl(url);
            Assert.Equal($"Регистрация - {Constants.OnlineShopName}", _webDriver.Title);
            Assert.Contains("Регистрация", _webDriver.PageSource);
        }

        [Theory]
        [InlineData("Николай","Шилин","Max","Max@gmail.com","Max", "1q2w3e4r", "1q2w3e4r", "Повторный пароль не указан")]
        [InlineData("Николай", "Шилин", "Max", "Max@gmail.com", "Max", "12", "", "Допускается от 3 до 20 символов")]
        public void Register_WrongModelData_ReturnsErrorMessage(string name, string serName, string nikName, string email, string login, string password, string passwordConfirm, string validationError)
        {
            _webDriver.Navigate().GoToUrl(url);
            _webDriver.FindElement(By.Name("Name")).SendKeys(name);
            _webDriver.FindElement(By.Name("SerName")).SendKeys(serName);
            _webDriver.FindElement(By.Name("NikName")).SendKeys(nikName);
            _webDriver.FindElement(By.Name("Email")).SendKeys(email);
            _webDriver.FindElement(By.Name("Login")).SendKeys(login);
            _webDriver.FindElement(By.Name("Password")).SendKeys(password);
            _webDriver.FindElement(By.Name("PasswordConfirm")).SendKeys(passwordConfirm);
            _webDriver.FindElement(By.ClassName("btn-dark")).Click();

            var errorMessage = _webDriver.FindElement(By.ClassName("field-validation-error")).Text;

            Assert.Equal(validationError, errorMessage);
        }
    }
}