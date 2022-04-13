using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CallTask.Models
{
    public class Phone : Person
    {
        private string _myNumber;
        public override string MyNumber
        {
            get { return _myNumber; }
            set
            {
                if (!String.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && value.Length == 10) _myNumber = value;
                else throw new Exception();
            }
        }
        public string ProviderName { get; set; }
        public double Balance { get; set; }

        private SoundPlayer soundPlayer;
        private List<Contact> Contacts;

        private Phone()
        {
            Contacts = new List<Contact>();
        }

        public Phone(string myNumber, double balance) : this()
        {
            SetProviderName(myNumber);
            MyNumber = myNumber;
            Balance = balance;
        }
        public bool OpenCallContact(string fullname)
        {
            bool isOk = true;
            int talkSecond = 0;
            Task task1 = new Task(() => isOk = CheckNoKey());
            Stopwatch _stopwatch = new Stopwatch();
            string counter = "";
            _stopwatch.Start();
            task1.Start();
        Start:
            isOk = CheckBalance(Balance);
            DecreaseBalance();
            while (isOk)
            {
                Task.WhenAny(task1);
                if (!isOk) break;
                counter = DateTime.Parse(_stopwatch.Elapsed.ToString()).ToString(@"mm\:ss");
                talkSecond++;
                PhoneCallUI("   " + fullname, counter);
                if (talkSecond == 59)
                {
                    talkSecond = 0;
                    goto Start;
                }
                Thread.Sleep(1000);
                Console.Clear();
            }
            _stopwatch.Stop();
            PhoneCallUI("Balans " + Balance.ToString(), counter);
            Console.Beep(329, 300); Console.Beep(329, 300); Console.Beep(329, 300);
            Thread.Sleep(1000);
            return isOk;
        }
        public bool OpenCallNumber(string num)
        {
            bool isOk = true;
            int talkSecond = 0;
            Task task1 = new Task(() => isOk = CheckNoKey());
            Stopwatch _stopwatch = new Stopwatch();
            string counter = "";
            _stopwatch.Start();
            task1.Start();
        Start:
            isOk = CheckBalance(Balance);
            DecreaseBalance();
            while (isOk)
            {
                Task.WhenAny(task1);
                if (!isOk) break;
                counter = DateTime.Parse(_stopwatch.Elapsed.ToString()).ToString(@"mm\:ss");
                talkSecond++;
                if (talkSecond == 59)
                {
                    talkSecond = 0;
                    goto Start;
                }
                PhoneCallUI(num, counter);
                Thread.Sleep(1000);
                Console.Clear();
            }
            _stopwatch.Stop();
            PhoneCallUI("Balans " + Balance.ToString(), counter);
            Console.Beep(329, 300); Console.Beep(329, 300); Console.Beep(329, 300);
            Thread.Sleep(1000);
            return isOk;
        }
        public bool Calling(Contact contact)
        {
            bool isAvailable = true;
            bool isOk = true;
            int count = 0;
            Stopwatch _stopwatch = new Stopwatch();
            isAvailable = CheckBalance(Balance);
            if (!isAvailable) ShowInfoBalance();
            Task task1 = new Task(() => isAvailable = CheckYesKey());
            task1.Start();
            Task task2 = new Task(() => isOk = CheckNoKey());
            task2.Start();
            _stopwatch.Start();
            string fileName = @"/Iphone.wav";
            soundPlayer = new SoundPlayer(Environment.CurrentDirectory + fileName);
            if (isAvailable)
                soundPlayer.Play();
            while (isAvailable && isOk)
            {
                Task.WhenAny(task1, task2);
                string counter = DateTime.Parse(_stopwatch.Elapsed.ToString()).ToString(@"mm\:ss");
                PhoneCallUI("Zəng gedir", counter);
                count++;
                Thread.Sleep(1000);
                Console.Clear();
                if (!isOk) { soundPlayer.Stop(); _stopwatch.Stop(); }
                if (!isAvailable)
                {
                    soundPlayer.Stop();
                    _stopwatch.Stop();
                    OpenCallContact(contact.Fullname);
                    break;
                }
                isAvailable = CheckCallingSeconds(count);
                if (!isAvailable)
                {
                    soundPlayer.Stop(); _stopwatch.Stop();
                }
            }
            return isAvailable;
        }

        public bool CallingNumber(string number)
        {
            bool isAvailable = true;
            bool isOk = true;
            int count = 0;
            Stopwatch _stopwatch = new Stopwatch();
            isAvailable = CheckBalance(Balance);
            if (!isAvailable) ShowInfoBalance();
            Task task1 = new Task(() => isAvailable = CheckYesKey());
            task1.Start();
            Task task2 = new Task(() => isOk = CheckNoKey());
            task2.Start();
            _stopwatch.Start();
            string fileName = @"/Iphone.wav";
            soundPlayer = new SoundPlayer(Environment.CurrentDirectory + fileName);
            if (isAvailable)
                soundPlayer.Play();
            while (isAvailable && isOk)
            {
                Task.WhenAny(task1, task2);
                string counter = DateTime.Parse(_stopwatch.Elapsed.ToString()).ToString(@"mm\:ss");
                PhoneCallUI("Zəng gedir", counter);
                count++;
                Thread.Sleep(1000);
                Console.Clear();
                if (!isOk) { soundPlayer.Stop(); _stopwatch.Stop(); }
                if (!isAvailable)
                {
                    soundPlayer.Stop();
                    _stopwatch.Stop();
                    OpenCallNumber(number);
                    break;
                }
                isAvailable = CheckCallingSeconds(count);
                if (!isAvailable) { soundPlayer.Stop(); _stopwatch.Stop(); }
            }
            return isAvailable;
        }
        public void AddNumber(Contact contact)
        {
            if (CheckContactNumber(contact)) Contacts.Add(contact);
            else throw new Exception("Nömrə təyin olunmayıb");
        }
        public void Delete(int? id)
        {
            if (id == null) throw new ArgumentNullException();
            foreach (var item in Contacts)
            {
                if (item != null)
                    if (item.Id == id) Contacts.Remove(item);
            }
        }
        public void AddBalance(double balance)
        {
            Balance += balance;
        }
        public void SetProviderName(string myNumber)
        {
            if (String.IsNullOrEmpty(myNumber) || String.IsNullOrWhiteSpace(myNumber)) throw new NullReferenceException();

            if (myNumber.StartsWith("+994"))
            {
                if (myNumber[4] == '7' && myNumber[5] == '0' || myNumber[5] == '7') ProviderName = "Nar";
                else if (myNumber[4] == '5' && myNumber[5] == '5' || myNumber[4] == '9' && myNumber[5] == '0') ProviderName = "Bakcell";
                else if (myNumber[4] == '5' && myNumber[5] == '0' || myNumber[5] == '1' || myNumber[4] == '1' && myNumber[5] == '0') ProviderName = "Azercell";
                else if (myNumber[4] == '6' && myNumber[5] == '0') ProviderName = "Naxtel";
                else Console.WriteLine("Nömrəni düzgün daxil edin");
            }
            else
            {
                if (myNumber[0] == '0' && myNumber[1] == '7' && myNumber[2] == '0' || myNumber[2] == '7') ProviderName = "Nar";
                else if (myNumber[0] == '0' && myNumber[1] == '5' && myNumber[2] == '5' || myNumber[1] == '9' && myNumber[2] == '0') ProviderName = "Bakcell";
                else if (myNumber[0] == '0' && myNumber[1] == '5' && myNumber[2] == '0' || myNumber[1] == '5' || myNumber[2] == '1' && myNumber[2] == '0') ProviderName = "Azercell";
                else if (myNumber[0] == '0' && myNumber[1] == '6' && myNumber[2] == '0') ProviderName = "Naxtel";
                else Console.WriteLine("Nömrəni düzgün daxil edin");
            }
        }
        public void CreditBalance(string symbol)
        {
            if (ProviderName == "Nar")
            {
                if (symbol == "*700#") { Balance += 3; BalanceInfo(); }
                else Console.WriteLine("USSD xətası");
            }
            else if (ProviderName == "Bakcell")
            {
                if (symbol == "*150#") { Balance += 2; BalanceInfo(); }
                else Console.WriteLine("USSD xətası");
            }
            else if (ProviderName == "Azercell")
            {
                if (symbol == "*100#") { Balance += 0.5; BalanceInfo(); }
                else Console.WriteLine("USSD xətası");
            }
            else if (ProviderName == "Naxtel")
            {
                if (symbol == "*100*1#") { Balance += 1; BalanceInfo(); }
                else Console.WriteLine("USSD xətası");
            }
        }
        public bool CheckContactNumber(Contact contact)
        {
            if (!String.IsNullOrEmpty(contact.Number) && MyNumber != contact.Number)
            {
                Regex regex = new Regex(@"^(\+994|0)(77|51|50|55|70|99|10|60)(\-|)(\d){3}(\-|)(\d){2}(\-|)(\d){2}$");
                if (regex.IsMatch(contact.Number)) return true;
            }
            return false;
        }
        public bool CheckMyNumber(string myNumber)
        {
            if (!String.IsNullOrEmpty(myNumber) || !String.IsNullOrWhiteSpace(myNumber))
            {
                Regex regex = new Regex(@"^(\+994|0)(77|51|50|55|70|99|10|60)(\-|)(\d){3}(\-|)(\d){2}(\-|)(\d){2}$");
                if (regex.IsMatch(myNumber)) return true;
            }
            return false;
        }
        public void BalanceInfo()
        {
            Console.WriteLine($"Balans: {Balance} Azn");
        }
        public void DecreaseBalance()
        {
            if (ProviderName == "Nar")
                Balance = Math.Round(Balance - 0.07, 2);
            else if (ProviderName == "Azercell")
                Balance = Math.Round(Balance - 0.08, 2);
            else if (ProviderName == "Bakcell")
                Balance = Math.Round(Balance - 0.10, 2);
            else if (ProviderName == "Naxtel")
                Balance = Math.Round(Balance - 0.9, 2);
        }
        public bool CheckYesKey()
        {
            yes:
            if (Console.ReadKey().Key == ConsoleKey.Y)
                return false;
            else goto yes;
        }
        public bool CheckNoKey()
        {
            no:
            if (Console.ReadKey().Key == ConsoleKey.N)
                return false;
            else goto no;
        }
        public bool CheckCallingSeconds(int count)
        {
            bool isAvailable = true;
            if (count >= 13)
            {
                isAvailable = false;
                PhoneCallDefuseUI("Zəng bitdi");
                Console.Beep(698, 300); Console.Beep(698, 300); Console.Beep(698, 300);
            }
            return isAvailable;
        }
        public bool CheckBalance(double balance)
        {
            if (ProviderName == "Nar")
                if (balance >= 0.07) return true;
            if (ProviderName == "Azercell")
                if (balance >= 0.08) return true;
            if (ProviderName == "Bakcell")
                if (balance >= 0.1) return true;
            if (ProviderName == "Naxtel")
                if (balance >= 0.09) return true;
            return false;
        }
        public void ShowInfo(Contact contact, int count)
        {
            Console.WriteLine($@"-------{contact.Fullname}------- {count} 
-----Zəng çalınır-----
Enter - Cavab
Space - Söndür");
        }
        public void ShowInfoBalance()
        {
            Console.WriteLine("Balansınızda kifayət qədər məbləğ yoxdur");
        }
        public Contact GetContact(int? id)
        {
            if (id == null) throw new ArgumentNullException();
            foreach (var item in Contacts)
            {
                if (item.Id == id)
                    return item;
            }
            throw new Exception();
        }
        public List<Contact> GetContacts()
        {
            return Contacts;
        }
        public void PhoneCallUI(string info, string count)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetWindowSize(35, 35);
            var date = DateTime.Now.ToShortTimeString();
            Console.CursorSize = 4;
            Console.WriteLine(@$" 
    ┌───────────────────────┐
    │                4G ╲|╱ │       
    │ {ProviderName}           
    │                       │
    │        {date}        │
    │      ───────────      │
    │                       │
    │      {info}                  
    │                       │
    │         {count}         │               
    │                       │
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
        public void PhoneCallDefuseUI(string info)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetWindowSize(35, 35);
            var date = DateTime.Now.ToShortTimeString();
            Console.CursorSize = 4;
            Console.WriteLine(@$" 
    ┌───────────────────────┐
    │                4G ╲|╱ │       
    │ {ProviderName}           
    │                       │
    │        {date}       │
    │      ───────────      │
    │                       │
    │                       │
    │       {info}      │
    │                       │        
    │                       │
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
    }
}
