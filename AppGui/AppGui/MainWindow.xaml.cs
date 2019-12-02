using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Xml.XPath;
using mmisharp;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Microsoft.Speech.Synthesis;
using multimodal;
using System.Globalization;

namespace AppGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MmiCommunication mmiC;
      
        
        //private bool orderDone = false;
        private bool orderStart = false;
        public TimeSpan MyDefaultTimeout { get; private set; }
        private Tts t;
        private ChromeDriver driver;
        private string[] commandsNotUnderstand = new string[5];
        private Random rnd = new Random();
        private bool cartClicked = false, leaving = false;

        private MmiEventArgs e;

        public MainWindow()
        {

            commandsNotUnderstand[0] = "Desculpe, pode repetir?";
            commandsNotUnderstand[1] = "Parece que estou a ficar surda, importa-se de repetir?";
            commandsNotUnderstand[2] = "Pode repetir o que disse?";
            commandsNotUnderstand[3] = "Não queria ser inconveniente, mas pode repetir?";
            commandsNotUnderstand[4] = "Não percebi o que disse.";
            t = new Tts();

            driver = new ChromeDriver();
            
            
            //t.Speak("2");


            openUberEatsChrome(driver);

            mmiC = new MmiCommunication("localhost", 8000, "User1", "GUI");
            mmiC.Message += MmiC_Message;
            //t.Speak("3");
            mmiC.Start();

            //t.Speak("4");
            Console.WriteLine("before driver...");
            
            Console.WriteLine("after driver function...");

            //t.Speak("coccocosocoasodasdas");
            //t.Speak("5");

            //there is something wrogn with the t.Speak
            //maybe assync


            //t.Speak("olá, tudo");


        }

        private void MmiC_Message(object sender, MmiEventArgs e)
        {
            Console.Write("A ATUALIZAR?");
            Console.Write(e.Message.ToString());
            this.e = e;
            //Console.WriteLine(e.Message);
            var doc = XDocument.Parse(e.Message);
            
            var com = doc.Descendants("command").FirstOrDefault().Value;
            dynamic json = JsonConvert.DeserializeObject(com);


            double confidence = double.Parse(json.recognized[0].ToString());
            
            WebDriverWait wait;

            //Console.WriteLine(json.recognized[2].ToString());

            

            var confirmation = (string)json.recognized[1].ToString();
            switch (confirmation) //confimation
            {
                case "AvancarL":
                    break;
                case "esvaziarC":
                    t.Speak("Ok, até breve!");
                    driver.Close();
                    System.Environment.Exit(1);
                    break;
                case "RecuarR":
                    leaving = false;
                    break;
                case "ScrollDR":
                    scrollSmooth();
                    break;
                case "ScrollU":
                    scrollSmooth();
                    break;
                default:
                    t.Speak("Não percebi");
                    break;
            } 

            if (orderStart && confidence > 0.65)
            {
                Console.WriteLine(confidence);
                string action;
                switch ((string)json.recognized[2].ToString())
                {

                    case "scroll":
                        scrollSmooth();
                        break;
                    case "search":
                        t.Speak("O que deseja procurar?");
                        break;
                    case "return":
                        var str = "https://www.ubereats.com/pt-PT/feed/?d=" + DateTime.Now.ToString("yyyy-M-dd") + "&et=870&pl=JTdCJTIyYWRkcmVzcyUyMiUzQSUyMkRFVEklMjAtJTIwRGVwYXJ0YW1lbnRvJTIwZGUlMjBFbGVjdHIlQzMlQjNuaWNhJTJDJTIwVGVsZWNvbXVuaWNhJUMzJUE3JUMzJUI1ZXMlMjBlJTIwSW5mb3JtJUMzJUExdGljYSUyMiUyQyUyMnJlZmVyZW5jZSUyMiUzQSUyMkNoSUpzVjdhcjZxaUl3MFJidHRlelhxZVI3YyUyMiUyQyUyMnJlZmVyZW5jZVR5cGUlMjIlM0ElMjJnb29nbGVfcGxhY2VzJTIyJTJDJTIybGF0aXR1ZGUlMjIlM0E0MC42MzMxNzMxMDAwMDAwMSUyQyUyMmxvbmdpdHVkZSUyMiUzQS04LjY1OTQ5MzMlN0Q%3D&ps=1&st=840";

                        driver.Navigate().GoToUrl(str);
                        break;

                    case "addtocart":
                        //var numero = driver.FindElementsByXPath("//div[@class='b5 b6 b7 b8 b9 c3 fw bp']"); // Recebe numero de items. driver.FindElement retorna objecto System...
                        /*var numero = driver.FindElementsByXPath("//descendant::div[@class='al an bo']"); // Recebe numero de items. driver.FindElement retorna objecto System...
                        action = "Adiciona " + numero + " ao pedido";
                        driver.FindElementByXPath("//parent::*[contains(text(), '" + action + "')]").Click();*/
                        
                        action = "Adiciona";
                        driver.FindElementByXPath("//parent::*[contains(text(), '" + action + "') and contains(text(), 'ao pedido')]").Click();

                        t.Speak("Adicionado ao carrinho com sucesso!");


                        break;

                    /*case "removefromcart":
                        action = "remove";
                        driver.FindElementByXPath("//parent::*[contains(text(), '" + action + "') and contains(text(), 'ao pedido')]").Click();
                        break;*/

                    /** XPATH returna null por algum motivo **/
                    /*case "reduceitem":
                        driver.FindElementByXPath("//div[@class='al an bo']/button[1]").Click();
                        break;
                    case "increaseitem":
                        //var test = driver.FindElementByXPath("//div[@class='al an bo']//descendant::button[position()=2]");
                        //test.Click();
                        
                        var test = driver.FindElements(By.CssSelector("button[class='b5 b6 b7 b8 b9 ba az eh fs ft fc al aj fb bf cd an bo ce fu fv']"));
                        test[1].Click();
                        break;
                        */

                    case "changedate":
                        action = "Entregar agora";
                        //driver.FindElementByXPath("//parent::*[contains(text(), '" + action + "')]").Click();
                        driver.FindElement(By.CssSelector("button[class='ao aq bi bj bk ah b2'")).Click();
                        action = "Agendar para mais tarde";
                        driver.FindElementByXPath("//parent::*[contains(text(), '" + action + "')]").Click();

                        // Adicionar switch para opções
                        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                        action = "Definir hora de entrega";
                        wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(text(), '" + action + "')]")));
                        driver.FindElementByXPath("//button[contains(text(), '" + action + "')]").Click();
                        driver.Navigate().Refresh();

                        t.Speak("Ok, a data de entrega foi alterada!");
                        break;

                    case "viewcart":
                        //driver.Navigate().Refresh();
                        cartClicked = !cartClicked;
                        if (!cartClicked)
                        {
                            driver.FindElementByXPath("//button[@aria-label='checkout']").Click();
                        }
                        t.Speak("Aqui tem o seu carrinho!");
                        break;
                    case "closecart":
                        driver.FindElementByCssSelector("button[class='af eh ei ej ek el em ao aq dt b2']").Click();
                        break;
                    default:
                        break;
                }
                
                switch ((string)json.recognized[3].ToString()) //restaurants
                {

                    case "MCDONALDS":
                        //search mcdonalds
                        driver.FindElementByXPath("//parent::*[contains(text(), 'Procurar')]").Click();
                        //searchBox.Click();
                        //var searchBox = driver.FindElement(By.CssSelector("input[class='bn ct']"));
                        var searchBox = driver.FindElementByXPath("//input[@placeholder='O que deseja?']");
                        for (int i = 0; i < 20; i++)
                        {
                            searchBox.SendKeys(Keys.Backspace);
                        }
                        searchBox.SendKeys("mcdonalds ");

                        string place;

                        switch ((string)json.recognized[4].ToString()) //place
                        {
                            case "UNIVERSIDADE":
                                place = "(Aveiro Universidade)";
                                searchBox.SendKeys("universidade");
                                searchBox.SendKeys(Keys.Enter);

                                //t.Speak("Carregue no botão");

                                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("div[class='bz c4 bx c0 c3']")));
                                //var txtElement = driver.FindElementsByXPath("[contains(text(), 'Universidade')]");
                                //var item = driver.FindElement(By.CssSelector("div[class='bz c4 bx c0 c3']"));
                                //item.Click();
                                driver.FindElementByXPath("//div/*[contains(text(), '" + place + "')]").Click();
                                break;
                            case "FORUM":
                                place = "(Aveiro Fórum)";
                                searchBox.SendKeys("fórum");
                                searchBox.SendKeys(Keys.Enter);

                                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("div[class='bz c4 bx c0 c3']")));
                                driver.FindElementByXPath("//div/*[contains(text(), '" + place + "')]").Click();
                                break;
                            case "GLICINIAS":
                                place = "(Aveiro Glicinias)";
                                searchBox.SendKeys("glicinias");
                                searchBox.SendKeys(Keys.Enter);

                                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("div[class='bz c4 bx c0 c3']")));
                                driver.FindElementByXPath("//div/*[contains(text(), '" + place + "')]").Click();
                                break;
                            case "":
                                searchBox.SendKeys(Keys.Enter);
                                //tts escolha a opção desejada
                                break;
                        }
                        
                        if ((string)json.recognized[4].ToString() == "")
                        {
                            t.Speak("De qual?");
                        } else
                        {
                            t.Speak("Que produto quer adquirir?");
                        }
                        
                        break;
                    case "MONTADITOS":
                        driver.FindElementByXPath("//parent::*[contains(text(), 'Procurar')]").Click();
                        searchBox = driver.FindElementByXPath("//input[@placeholder='O que deseja?']");
                        for (int i = 0; i < 20; i++)
                        {
                            searchBox.SendKeys(Keys.Backspace);
                        }
                        searchBox.SendKeys("100 montaditos ");  

                        searchBox.SendKeys(Keys.Enter);

                        place = "100 Montaditos";

                        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                        wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("div[class='bz c4 bx c0 c3']")));
                        driver.FindElementByXPath("//div/*[contains(text(), '" + place + "')]").Click();
                        break;
                    case "PIZZAHUT":
                        driver.FindElementByXPath("//parent::*[contains(text(), 'Procurar')]").Click();
                        searchBox = driver.FindElementByXPath("//input[@placeholder='O que deseja?']");
                        for (int i = 0; i < 20; i++)
                        {
                            searchBox.SendKeys(Keys.Backspace);
                        }
                        searchBox.SendKeys("pizza hut ");

                        /*switch ((string)json.recognized[4].ToString()) //place
                        {
                            case "UNIVERSIDADE":
                                searchBox.SendKeys("universidade");
                                break;
                            case "FORUM":
                                searchBox.SendKeys("fórum");
                                break;
                            case "GLICINIAS":
                                searchBox.SendKeys("glicinias");
                                break;
                        }*/
                        searchBox.SendKeys(Keys.Enter);


                        place = "Pizza Hut";

                        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                        wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("div[class='bz c4 bx c0 c3']")));
                        driver.FindElementByXPath("//div/*[contains(text(), '" + place + "')]").Click();
                        break;
                }

                //pergunta ao user o que quer?



                switch ((string)json.recognized[5].ToString()) //options
                {

                    case "":
                        break;
                    case ".":
                        //search mcdonalds
                        //pergunta ao user o que quer?
                        break;
                    case "-":

                        break;
                    case "-.":

                        break;
                }
                
                switch ((string)json.recognized[6].ToString()) //food on mcdonalds
                {
                    case "":
                        break;
                    default:

                        var food = (string)json.recognized[6].ToString();
                        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                        wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//*[contains(text(), '" + food + "')]")));

                        driver.FindElementByXPath("//*[contains(text(), '" + food + "')]").Click();
                        t.Speak("Deseja alterar o seu pedido?");
                        break;
                }
                

                IWebElement element;

                string itemName = "";
                switch ((string)json.recognized[7].ToString()) //food on mcdonalds
                {
                    case "":
                        break;
                    default:
                        itemName = (string)json.recognized[7].ToString();

                        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                        wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//*[contains(text(), '" + itemName + "')]")));

                        element = driver.FindElementByXPath("//*[contains(text(), '" + itemName + "')]");
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
                        System.Threading.Thread.Sleep(500);

                        driver.FindElementByXPath("//*[contains(text(), '" + itemName + "')]").Click();
                        break;
                }

            }
        }

        public void scrollSmooth()
        {
            while(true)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0,1)", "");
                //le o array

                mmiC.Message += MmiC_Message;
                mmiC.Start();
                //t.Speak("3");
                
                
                var doc = XDocument.Parse(e.Message);

                var com = doc.Descendants("command").FirstOrDefault().Value;
                dynamic json = JsonConvert.DeserializeObject(com);


                //Console.Write(mmiC.ToString());
                
                //Console.Write("im here");

                if ((string)json.recognized[1].ToString() == "StopS")
                    break;
                
            }
        }

        public static void openUberEatsChrome(ChromeDriver driver)
        {
            // Initialize the Chrome Driver
            //using (driver)
            //{
            // 1. Maximize the browser
            driver.Manage().Window.Maximize();

            // 2. Go to the "Google" homepage
            /*driver.Navigate().GoToUrl("https://www.ubereats.com/pt-PT/feed/?pl=JTdCJTIyYWRkcmVzcyUyMiUzQSUyMkRFVEklMjAtJTIwRGVwYXJ0YW1lbnRvJTIwZGUlMjBFbGVjdHIlQzMlQjNuaWNhJTJDJTIwVGVsZWNvbXVuaWNhJUMzJUE3JUMzJUI1ZXMlMjBlJTIwSW5mb3JtJUMzJUExdGljYSUyMiUyQyUyMnJlZmVyZW5jZSUyMiUzQSUyMkNoSUpzVjdhcjZxaUl3MFJidHRlelhxZVI3YyUyMiUyQyUyMnJlZmVyZW5jZVR5cGUlMjIlM0ElMjJnb29nbGVfcGxhY2VzJTIyJTJDJTIybGF0aXR1ZGUlMjIlM0E0MC42MzMxNzMxMDAwMDAwMSUyQyUyMmxvbmdpdHVkZSUyMiUzQS04LjY1OTQ5MzMlN0Q%3D");
            */

            var str = "https://www.ubereats.com/pt-PT/feed/?d=" + DateTime.Now.ToString("yyyy-M-dd") + "&et=870&pl=JTdCJTIyYWRkcmVzcyUyMiUzQSUyMkRFVEklMjAtJTIwRGVwYXJ0YW1lbnRvJTIwZGUlMjBFbGVjdHIlQzMlQjNuaWNhJTJDJTIwVGVsZWNvbXVuaWNhJUMzJUE3JUMzJUI1ZXMlMjBlJTIwSW5mb3JtJUMzJUExdGljYSUyMiUyQyUyMnJlZmVyZW5jZSUyMiUzQSUyMkNoSUpzVjdhcjZxaUl3MFJidHRlelhxZVI3YyUyMiUyQyUyMnJlZmVyZW5jZVR5cGUlMjIlM0ElMjJnb29nbGVfcGxhY2VzJTIyJTJDJTIybGF0aXR1ZGUlMjIlM0E0MC42MzMxNzMxMDAwMDAwMSUyQyUyMmxvbmdpdHVkZSUyMiUzQS04LjY1OTQ5MzMlN0Q%3D&ps=1&st=840";

            driver.Navigate().GoToUrl(str);

            // 3. Find the search textbox (by ID) on the homepage


            //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            //wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("button[class='ao aq b2']")));


            //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            //driver.Manage().Window.Minimize();
            //driver.Manage().Window.Maximize();
            //driver.FindElement(By.CssSelector("button[class='ao aq b2']")).Click(); //By.CssSelector("button[class='ao aq b2]'"));
                                                                                    //driver.FindElementByCssSelector("button.ao.aq.b2");
                                                                                    //searchBox.Click();
                                                                                    //searchBox.SendKeys(Keys.Enter);
                                                                                    //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            //wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            //wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("#search-suggestions-input")));






            /*

            new WebDriverWait(driver, MyDefaultTimeout).Until(
            d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));


            searchBox.SendKeys("DETI");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("location-enter-address-item-0")));

            searchBox.SendKeys(Keys.Enter);
            */

            //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            //wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("location-enter-address-item-0")));

            //IWebElement deti = driver.FindElementByCssSelector("button[type='submit']");
            //deti.Click();
            // 6. Click "Submit" to start the search
            //searchBox.SendKeys(Keys.Enter);

            // 7. Find the "Id" of the "Div" containing results stats
            //var searchResults = driver.FindElementById("resultStats");

            //Console.ReadKey();
            // }
        }
        
    }
}
