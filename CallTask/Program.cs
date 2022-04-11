using CallTask.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace CallTask
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.ForegroundColor = ConsoleColor.White;

            string myNumber = "";
            double balance = 0;
            string fullname = "";
            string number = "";
            int? id = 0;
            int? choise = 0;
        SetPhoneInputs:
            SetPhoneInputs(ref myNumber, ref balance);
            Phone phone = new Phone(myNumber, balance);
            bool isOk = phone.CheckBalance(balance);
            if (!isOk)
            {
                Console.WriteLine("Düzgün balans daxil edin");
                goto SetPhoneInputs;
            }
        Start:
            do
            {
                Console.SetWindowSize(120, 30);
                Thread.Sleep(500);
                ShowInfo();
                SetChoice(ref choise);
                switch (choise)
                {
                    case 0:
                        Console.WriteLine("Çıxış edildi");
                        break;
                    case 1:
                        SetContactInputs(ref fullname, ref number);
                        Contact contact = new Contact(fullname, number);
                        phone.AddNumber(contact);
                        break;
                    case 2:
                    SetBalance:
                        SetBalance(ref balance);
                        isOk = phone.CheckBalance(balance);
                        if (isOk)
                            phone.AddBalance(balance);
                        else goto SetBalance;
                        break;
                    case 3:
                        try
                        {
                            Console.Write("Id daxil edin: ");
                            choise = Convert.ToInt32(Console.ReadLine());
                            phone.GetContact(choise);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Bu id-li şəxs kontaktda mövcud deyil");
                            goto Start;
                        }
                        Contact contact1 = phone.GetContact(choise);
                        phone.Calling(contact1);
                        break;
                    case 4:
                        GetContacts(phone.GetContacts());
                        break;
                    case 5:
                        isOk = KeyPad(out StringBuilder sb, phone.ProviderName);
                        if (!isOk)
                            phone.CallingNumber(sb.ToString());
                        else phone.CreditBalance(sb.ToString());
                        break;
                    case 6:
                        SetId(ref id);
                        DeleteContact(id, phone.GetContacts());
                        break;
                    case 7:
                        phone.BalanceInfo();
                        break;
                    case 8:
                        Console.Clear();
                        break;
                    default:
                        Console.WriteLine("Menyudan kənar rəqəm daxil etməyin");
                        break;
                }
            } while (choise != 0);
        }

        #region Inputs
        static string myNum;
        static void SetPhoneInputs(ref string myNumber, ref double balance)
        {
        SetMyNumber:
            try
            {
                Console.Write("Nömrənizi daxil edin: ");
                myNumber = Console.ReadLine();
                myNum = myNumber;
                Regex regex = new Regex(@"^(\+994|0)(77|51|50|55|70|99|10|60)(\-|)(\d){3}(\-|)(\d){2}(\-|)(\d){2}$");
                if (!regex.IsMatch(myNumber))
                    throw new Exception();
            }
            catch (Exception)
            {
                Console.WriteLine("Nömrəni düzgün daxil edin");
                goto SetMyNumber;
            }
        SetBalance:
            try
            {
                Console.Write("Balans daxil edin: ");
                balance = Convert.ToDouble(Console.ReadLine());
                if (balance < 0 || balance > 1000)
                    throw new Exception();
            }
            catch (FormatException)
            {
                Console.WriteLine("Ancaq rəqəm simvolu daxil edə bilərsiniz");
                goto SetBalance;
            }
            catch (Exception) { Console.WriteLine("Info (0-1000) aralığı"); goto SetBalance; }
        }
        static void SetContactInputs(ref string fullName, ref string number)
        {
        SetFullName:
            try
            {
                Console.Write("Ad daxil edin: ");
                fullName = Console.ReadLine();
                if (String.IsNullOrEmpty(fullName) || String.IsNullOrWhiteSpace(fullName))
                    throw new Exception();
            }
            catch (Exception)
            {
                Console.WriteLine("Ad daxil etmək məcburidir");
                goto SetFullName;
            }
        SetNumber:
            try
            {
                Console.Write("Nömrə daxil edin: ");
                number = Console.ReadLine();
                Regex regex = new Regex(@"^(\+994|0)(77|51|50|55|70|99|10|60)(\-|)(\d){3}(\-|)(\d){2}(\-|)(\d){2}$");
                if (!regex.IsMatch(number) || myNum == number)
                    throw new Exception();
            }
            catch (Exception)
            {
                Console.WriteLine("Nömrəni düzgün daxil edin");
                goto SetNumber;
            }
        }
        static void SetBalance(ref double balance)
        {
        SetBalance:
            try
            {
                Console.Write("Balans daxil edin: ");
                balance = Convert.ToDouble(Console.ReadLine());

                if (balance < 0 || balance > 1000)
                    throw new Exception();
            }
            catch (FormatException)
            {
                Console.WriteLine("Ancaq rəqəm simvolu daxil edə bilərsiniz");
                goto SetBalance;
            }
            catch (Exception) { Console.WriteLine("Info (0-1000) aralığı"); goto SetBalance; }
        }
        static void SetChoice(ref int? choice)
        {
        start:
            try
            {
                Console.Write("Seçim edin: ");
                choice = Convert.ToInt32(Console.ReadLine());
                choice.ToString().Trim();
                if (choice < 0) throw new Exception();
            }
            catch (FormatException)
            {
                Console.WriteLine("Ancaq rəqəm daxil edin");
                goto start;
            }
            catch (Exception) { Console.WriteLine("Id menfi ola bilmez"); goto start; }
        }
        static void CharInput(ref char choise)
        {
        Start:
            try
            {
                Console.WriteLine(@"       
┌───────────────────────────────┐
│────────────Kredit─────────────│
│ Nar - *700# - 3 Azn           │
│ Bakcell - *150# - 2 Azn       │
│ Azercell - *100# - 0.50 Azn   │
│ Naxtel - *100*1# - 1 Azn      │
└───────────────────────────────┘");
                Console.Write("Rəqəm simvolu daxil edin: ");
                choise = Convert.ToChar(Console.ReadLine());
            }
            catch (Exception)
            {
                Console.WriteLine();
                goto Start;
            }
        }
        #endregion

        #region Additional
        static void SetId(ref int? id)
        {
        start:
            try
            {
                Console.Write("Id daxil edin: ");
                id = Convert.ToInt32(Console.ReadLine());
                id.ToString().Trim();
                if (id < 0) throw new Exception();
            }
            catch (FormatException)
            {
                Console.WriteLine("Ancaq rəqəm daxil edin");
                goto start;
            }
            catch (Exception) { Console.WriteLine("Id menfi ola bilmez"); goto start; }
        }
        static void ShowInfo()
        {
            Console.WriteLine(@"0 - Çıxış et
1 - Kontakta əlavə et
2 - Balans əlavə et
3 - Zəng et
4 - Kontaktlar
5 - Nömrəyə zəng et
6 - Kontaktı sil
7 - Balansı öyrən
8 - Konsolu təmizlə
");
        }
        static void GetContacts(List<Contact> list)
        {
            foreach (var item in list)
                if (item != null) Console.WriteLine($@"Id: {item.Id} | Ad: {item.Fullname} | Nömrə: {item.Number}");
        }
        static void DeleteContact(int? id, List<Contact> contacts)
        {
            try
            {
                if (id == null) throw new ArgumentNullException();
                foreach (var item in contacts)
                {
                    if (item != null)
                        if (item.Id == id)
                        {
                            Console.WriteLine($"{item.Fullname} kontaktdan silindi");
                            contacts.Remove(item);
                            return;
                        }
                }
                throw new Exception();
            }
            catch (Exception)
            {
                Console.WriteLine("Bu id movcud deyil");
            }
        }
        static bool KeyPad(out StringBuilder numberInput, string p)
        {
            numberInput = new StringBuilder();
            bool isOk = true;
            bool isTrue = true;
            char choise = ' ';
            {
                PhoneUI(numberInput, p);
                while (numberInput.Length <= 13 && isOk && isTrue)
                {
                    CharInput(ref choise);
                    switch (choise)
                    {
                        case '1':
                            numberInput.Append("1");
                            Console.Beep(783, 300);
                            Console.Clear();
                            break;
                        case '2':
                            numberInput.Append("2");
                            Console.Beep(783, 300);
                            Console.Clear();
                            break;
                        case '3':
                            numberInput.Append("3");
                            Console.Beep(783, 300);
                            Console.Clear();
                            break;
                        case '4':
                            numberInput.Append("4");
                            Console.Beep(783, 300);
                            Console.Clear();
                            break;
                        case '5':
                            numberInput.Append("5");
                            Console.Beep(783, 300);
                            Console.Clear();
                            break;
                        case '6':
                            numberInput.Append("6");
                            Console.Beep(783, 300);
                            Console.Clear();
                            break;
                        case '7':
                            numberInput.Append("7");
                            Console.Beep(783, 300);
                            Console.Clear();
                            break;
                        case '8':
                            numberInput.Append("8");
                            Console.Beep(783, 300);
                            Console.Clear();
                            break;
                        case '9':
                            Console.Clear();
                            Console.Beep(783, 300);
                            numberInput.Append("9");
                            break;
                        case '0':
                            Console.Clear();
                            Console.Beep(783, 300);
                            numberInput.Append("0");
                            break;
                        case '*':
                            Console.Clear();
                            Console.Beep(783, 300);
                            numberInput.Append("*");
                            break;
                        case '#':
                            Console.Clear();
                            Console.Beep(783, 300);
                            numberInput.Append("#");
                            break;
                        case '+':
                            Console.Clear();
                            Console.Beep(783, 300);
                            numberInput.Append("+");
                            break;
                        case '-':
                            numberInput.Clear();
                            break;
                        case 'Y':
                            isOk = false;
                            break;
                        case 'N':
                            return true;
                        default:
                            Console.Clear();
                            break;
                    }
                    PhoneUI(numberInput, p);
                }
                if (numberInput.ToString() == "*700#") return true;
                else if (numberInput.ToString() == "*150#") return true;
                else if (numberInput.ToString() == "*100#") return true;
                else if (numberInput.ToString() == "*100*1#") return true;

                Regex regex = new Regex(@"^(\+994|0)(77|51|50|55|70|99|10|60)(\-|)(\d){3}(\-|)(\d){2}(\-|)(\d){2}$");
                if (regex.IsMatch(numberInput.ToString()))
                {
                    numberInput.ToString();
                    return isOk;
                }
                Console.WriteLine("Nömrə yanlış daxil edildi");
                return true;
            }
        }
        static void PhoneUI(StringBuilder num, string p)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetWindowSize(35, 35);
            var date = DateTime.Now.ToShortTimeString();
            Console.WriteLine(@$" 
    ┌───────────────────────┐
    │                   ╲|╱ │
    │ {p}                 
    │                       │
    │        {date}        
    │      ───────────      │
    │                       │
    │                       │
    │                       │
    │                       │
    │                       │
            {num}      
    │───────────────────────│
    │───────┐   ↑   ┌───────│
    │   Y   │ ← + → │   N   │
    │───────┘   ↓   └───────│
    │───────────────────────│
    │   1   │   2   │   3   │
    │───────────────────────│
    │   4   │   5   │   6   │
    │───────────────────────│
    │   7   │   8   │   9   │
    │───────────────────────│
    │   *   │   0   │   #   │
    └───────────────────────┘");
        }
        #endregion
    }

}
