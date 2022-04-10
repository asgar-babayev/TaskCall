using CallTask.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;

namespace CallTask
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

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
                SetChoice(ref choise);
                switch (choise)
                {
                    case 0:
                        ShowInfo();
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
                        isOk = KeyPad(out StringBuilder sb);
                        if (!isOk)
                            phone.CallingNumber(sb.ToString());
                        break;
                    case 6:
                        SetId(ref id);
                        DeleteContact(id, phone.GetContacts());
                        break;
                    case 7:
                        phone.BalanceInfo();
                        break;
                    default:
                        break;
                }
            } while (choise != 8);
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
                Console.Write("Seçim edin(0-Info): ");
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
                Console.Write("Nömrə üçün rəqəm simvolu daxil edin (Zəng üçün # simvolunu daxil edin): ");
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
            Console.WriteLine(@"0 - İnformasiya
1 - Kontakta əlavə et
2 - Balans əlavə et
3 - Zəng et
4 - Kontaktlar
5 - Nömrəyə zəng et
6 - Kontaktı sil
7 - Balansı öyrən
8 - Çıxış
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
        static bool KeyPad(out StringBuilder numberInput)
        {
            numberInput = new StringBuilder();
            bool isOk = true;
            char choise = ' ';
            {
                KeyPadUI(numberInput);
                while (numberInput.Length <= 10 && isOk == true)
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
                        case '#':
                            isOk = false;
                            break;
                        case '*':
                            return true;
                        default:
                            break;
                    }
                    KeyPadUI(numberInput);

                }
                numberInput.ToString();
                return isOk;
            }
        }
        static void KeyPadUI(StringBuilder num)
        {
            Console.WriteLine(@$"
                    {num}
            ┌───────────────────────┐
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
