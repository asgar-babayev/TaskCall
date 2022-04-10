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
        public bool OpenCallContact()
        {
            bool isOk = true;
            int talkSecond = 0;
            Task task1 = new Task(() => isOk = CheckEnterKey());
            Stopwatch _stopwatch = new Stopwatch();
            string counter = "";
            _stopwatch.Start();
            task1.Start();
        Start:
            isOk = CheckBalance(Balance);
            DecrementBalance();
            while (isOk)
            {
                Task.WhenAny(task1);
                if (!isOk) break;
                counter = DateTime.Parse(_stopwatch.Elapsed.ToString()).ToString(@"mm\:ss");
                talkSecond++;
                Console.WriteLine(@$"Danışıq vaxtı: {counter}
Enter - Söndür");
                if (talkSecond == 56)
                {
                    talkSecond = 0;
                    goto Start;
                }
                Thread.Sleep(1000);
                Console.Clear();
            }
            _stopwatch.Stop();
            Console.Beep(329, 300); Console.Beep(329, 300); Console.Beep(329, 300);
            BalanceInfo();
            Console.WriteLine("Danışıq vaxtı: " + counter);
            return isOk;
        }
        public bool OpenCallNumber()
        {
            bool isOk = true;
            int talkSecond = 0;
            Task task1 = new Task(() => isOk = CheckEnterKey());
            Stopwatch _stopwatch = new Stopwatch();
            string counter = "";
            _stopwatch.Start();
            task1.Start();
        Start:
            isOk = CheckBalance(Balance);
            DecrementBalance();
            while (isOk)
            {
                Task.WhenAny(task1);
                if (!isOk) break;
                counter = DateTime.Parse(_stopwatch.Elapsed.ToString()).ToString(@"mm\:ss");
                talkSecond++;
                if (talkSecond == 56)
                {
                    talkSecond = 0;
                    goto Start;
                }
                Console.WriteLine(@$"Danışıq vaxtı: {counter}
Enter - Söndür");
                Thread.Sleep(1000);
                Console.Clear();
            }
            _stopwatch.Stop();
            Console.Beep(329, 300); Console.Beep(329, 300); Console.Beep(329, 300);
            BalanceInfo();
            Console.WriteLine("Danışıq vaxtı: " + counter);
            return isOk;
        }
        public bool Calling(Contact contact)
        {
            bool isAvailable = true;
            bool isOk = true;
            int count = 0;
            isAvailable = CheckBalance(Balance);
            if (!isAvailable) ShowInfoBalance();
            Task task1 = new Task(() => isAvailable = CheckEnterKey());
            task1.Start();
            Task task2 = new Task(() => isOk = CheckSpaceKey());
            task2.Start();
            string fileName = @"/Iphone.wav";
            soundPlayer = new SoundPlayer(Environment.CurrentDirectory + fileName);
            if (isAvailable)
                soundPlayer.Play();
            while (isAvailable && isOk)
            {
                Task.WhenAny(task1, task2);
                ShowInfo(contact, count);
                count++;
                Thread.Sleep(1000);
                Console.Clear();
                if (!isOk) { soundPlayer.Stop(); }
                if (!isAvailable)
                {
                    soundPlayer.Stop();
                    OpenCallContact();
                    break;
                }
                isAvailable = CheckCallingSeconds(count);
                if (!isAvailable) soundPlayer.Stop();
            }
            return isAvailable;
        }

        public bool CallingNumber(string number)
        {
            bool isAvailable = true;
            bool isOk = true;
            int count = 0;
            isAvailable = CheckBalance(Balance);
            if (!isAvailable) ShowInfoBalance();
            Task task1 = new Task(() => isAvailable = CheckEnterKey());
            task1.Start();
            Task task2 = new Task(() => isOk = CheckSpaceKey());
            task2.Start();
            string fileName = @"/Iphone.wav";
            soundPlayer = new SoundPlayer(Environment.CurrentDirectory + fileName);
            if (isAvailable)
                soundPlayer.Play();
            while (isAvailable && isOk)
            {
                Task.WhenAny(task1, task2);
                Console.WriteLine($@"-------{number}------- {count} 
-------Zəng çalınır-------
Enter - Cavab
Space - Söndür");
                count++;
                Thread.Sleep(1000);
                Console.Clear();
                if (!isOk) { soundPlayer.Stop(); }
                if (!isAvailable)
                {
                    soundPlayer.Stop();
                    OpenCallNumber();
                    break;
                }
                isAvailable = CheckCallingSeconds(count);
                if (!isAvailable) soundPlayer.Stop();
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
                else if (myNumber[4] == '5' && myNumber[5] == '5') ProviderName = "Bakcell";
                else if (myNumber[4] == '5' && myNumber[5] == '0' || myNumber[5] == '1') ProviderName = "Azercell";
                else throw new Exception("Nömrəni düzgün daxil edin");
            }
            else
            {
                if (myNumber[0] == '0' && myNumber[1] == '7' && myNumber[2] == '0' || myNumber[2] == '7') ProviderName = "Nar";
                else if (myNumber[0] == '0' && myNumber[1] == '5' && myNumber[2] == '5') ProviderName = "Bakcell";
                else if (myNumber[0] == '0' && myNumber[1] == '5' && myNumber[2] == '0' || myNumber[2] == '1') ProviderName = "Azercell";
                else throw new Exception("Nömrəni düzgün daxil edin");
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
        public void DecrementBalance()
        {
            if (ProviderName == "Nar")
                Balance = Math.Round(Balance - 0.07, 2);
            else if (ProviderName == "Azercell")
                Balance = Math.Round(Balance - 0.08, 2);
            else if (ProviderName == "Bakcell")
                Balance = Math.Round(Balance - 0.10, 2);
        }
        public bool CheckEnterKey()
        {
            if (Console.ReadKey().Key == ConsoleKey.Enter)

                return false;
            return true;
        }
        public bool CheckSpaceKey()
        {
            if (Console.ReadKey().Key == ConsoleKey.Spacebar)
                return false;
            return true;
        }
        public bool CheckCallingSeconds(int count)
        {
            bool isAvailable = true;
            if (count >= 13)
            {
                isAvailable = false;
                Console.Beep(698, 300); Console.Beep(698, 300); Console.Beep(698, 300);
                Console.WriteLine("Zəng bitdi");
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

    }
}
