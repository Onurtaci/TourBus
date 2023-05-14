using System;
using System.Collections.Generic;
using System.Linq;

namespace TourBus
{
    public class Passenger
    {
        public string Name; // Yolcu ismi
        public int SeatNumber; // Yolcunun oturduğu koltuk numarası, koltuk numarası en başta -1 atanıyor bunun sayesinde hangi yolcunun yerleşip yerleşmediğini kontrol edebiliyoruz
        public double SumDistance; //Yolcunun yerleştiğindeki komşu yolcularla olan uzaklıklar toplamı
        public int Index; // hangi koltuğa hangi yolcunun yerleştiğini anlayabilmek için tutulan, yolcu listesine göre olan index değeri

        public Passenger()
        {
            Name = null;
            SeatNumber = -1;
            SumDistance = Index = 0;
        }
    }

    internal class TourBus
    {
        static Random random = new Random();

        public static void Main(string[] args)
        {
            double[,] dm = CreateDistanceMatrices(); //Uzaklık matrisini oluşturup döndüren metot..
            Passenger[] passengerList = GetPassengerList(); // Yolcu listesini her bir elemanı Passenger sınıfının nesnesi olarak döndüren metot..
            Passenger[,] seatLayout = PlacePassenger(passengerList, dm); //Yolcuları gerekli koşula göre yerleştirip seatLayout yani koltuk düzeni matrisini döndüren metot.. 

            PrintSeatLayout(seatLayout); //Yolcuları koltuklarda Yolcu Numaraları ve Yolcu İsimleri olacak şekilde listeyen metot..
            double SumDistances = PrintSeatLayoutWithTotalDistance(seatLayout); // Yolcuların toplam uzaklıklarını koltukların üzerinde olacak şekilde listeleyen
                                                                                // ve tüm yolcuların toplam uzaklığını döndüren metot..
            Console.WriteLine("--> Tüm Yolcuların Uzaklıkları Toplamı: " + SumDistances.ToString("000.00"));

            Console.ReadKey();
        }

        private static double[,] CreateDistanceMatrices()
        {
            double[,] dm = new double[40, 40];
            for (int i = 0; i < dm.GetLength(0); i++)
            {
                for (int j = 0; j < i; j++) // uzaklık matrisi simetrik bir kare matris olduğundan matrisin
                                            // köşegeni ile ayrılan sol alt tarafına değer ataması yeterlidir.. 
                {
                    if (i == j) dm[i, j] = 0; // yolcunun kendisi ile olan uzaklığı sıfır olarak atanıyor..
                    else dm[i, j] = dm[j, i] = random.Next(1, 9) + random.NextDouble(); //iki yolcu arası uzaklık aynı olacağından
                                                                                        //hem [i,j] ye hem de [j,i] ye aynı değer atanıyor..
                }
            }
            return dm;
        }

        private static Passenger[] GetPassengerList()
        {
            string[] firstNames = { "Ali   ", "Ahmet ", "Merve ", "Elif  ", "Defne ", "Mert  ", "Bahar ", "Ezgi  " };
            string[] lastNames = { "Esen ", "Aksoy", "Çolak", "Dinç ", "Biçer" };
            string[] passengerNameList = new string[40];
            int index = 0;

            for (int i = 0; i < firstNames.Length; i++)
            {
                for (int j = 0; j < lastNames.Length; j++)
                    passengerNameList[index++] = firstNames[i] + lastNames[j];// Yolcuların ad soyadları farklı olacak şakilde atanıyor..
            }

            Passenger[] passengerList = new Passenger[passengerNameList.Length]; // Elemanları Passenger nesnesi olan bir dizi açılıyor..

            for (int i = 0; i < passengerNameList.Length; i++) // Yukarıda belirlenen isimler ve index değerleri dizideki her bir yolcuya atanıyor..
            {
                passengerList[i] = new Passenger();
                passengerList[i].Name = passengerNameList[i];
                passengerList[i].Index = i;
            }
            return passengerList;
        }

        private static Passenger[,] PlacePassenger(Passenger[] passengerList, double[,] dm)
        {
            int seatNum = 1;
            Passenger[,] seatLayout = new Passenger[10, 4]; // 10x4 lük bir Otobüs Koltuk Düzeni matrisi oluşturuluyor..
            List<double> tempDistsList = new List<double>(); // Her bir koltuğa yerleşebilecek olan yolcuların uzaklıklar toplamını tutacak olan liste yapısı.. 
            List<Passenger> tempPassengerList = new List<Passenger>(); // tempDistList listesine her uzaklıklar toplamı eklendiğinde o an işlem yapılan yolcuyu eklenen liste yapısı..

            for (int i = 0; i < seatLayout.GetLength(0); i++)
            {
                for (int j = 0; j < seatLayout.GetLength(1); j++)
                {
                    if (i == 0 && j == 0) // ilk yolcuyu random olarak yerleştirir..
                    {
                        Passenger firstPassenger = passengerList[random.Next(0, passengerList.Length)];
                        firstPassenger.SeatNumber = seatNum++;
                        seatLayout[0, 0] = firstPassenger; continue;
                    }

                    tempDistsList.Clear(); tempPassengerList.Clear(); // Her koltuk için sıfırlanıyor..

                    for (int k = 0; k < passengerList.Length; k++)
                    {
                        if (passengerList[k].SeatNumber != -1) continue; // Eğer yolcuya koltuk numarası atanmışsa yani default değeri olan -1 değilse o yolcuyu es geçiyor..

                        double dist; // Hangi koltuğa yerleşeceğine bağlı olarak hesaplanacak olan uzaklık(lar) toplamı değişkeni..
                        if (i == 0) // Eğer koltuk numarası 2,3 veya 4 ise..
                        {
                            dist = dm[seatLayout[i, j - 1].Index, k]; // oturan yolcu.Index ile o yolcunun yolcu listesindeki index'ine yani uzaklık matrisindeki gerekli index'e ulaşılıyor.. 
                        }
                        else // eğer koltuk numarası 5 veya üstü ise..
                        {
                            if (j == 0) // eğer otobüsün en sol tarafına yolcu yerleştirilecekse..  
                                dist = dm[passengerList[k].Index, seatLayout[i - 1, j].Index] +
                                       dm[passengerList[k].Index, seatLayout[i - 1, j + 1].Index];

                            else if (j == 3) // eğer otobüsün en sağ tarafına yolcu yerleştirilecekse..
                                dist = dm[passengerList[k].Index, seatLayout[i - 1, j - 1].Index] +
                                       dm[passengerList[k].Index, seatLayout[i - 1, j].Index] +
                                       dm[passengerList[k].Index, seatLayout[i - 1, j].Index];

                            else //eğer orta kısımlara yolcu yerleştirilcekse..
                                dist = dm[passengerList[k].Index, seatLayout[i - 1, j - 1].Index] +
                                       dm[passengerList[k].Index, seatLayout[i - 1, j].Index] +
                                       dm[passengerList[k].Index, seatLayout[i - 1, j + 1].Index] +
                                       dm[passengerList[k].Index, seatLayout[i, j - 1].Index];
                        }
                        tempDistsList.Add(dist); // yerleşmeyen yolcuların hepsinin yerleştirilecek koltuğa göre oluşturulan dist değişkeni listeye atılıyor.. 
                        tempPassengerList.Add(passengerList[k]); // üstteki dist değişkeni atanan yolcu listeye atılıyor..
                    }
                    double minDist = tempDistsList.Min(); // Koltuğa yerleşmesi gereken yolcunun uzaklık toplamı bulunuyor..
                    int minDistIndex = tempDistsList.IndexOf(minDist); // Yerleştirelecek yolcunun index i belirleniyor..
                    seatLayout[i, j] = tempPassengerList[minDistIndex]; // Koltuk düzenine yolcu yerleştiriliyor..
                    seatLayout[i, j].SeatNumber = seatNum++; // Ve yerleşen yolcuya koltuk numarası atanıp daha sonra koltuk numarası 1 arttırılıyor..
                    seatLayout[i, j].SumDistance = minDist; // Yerleşen yolcunun hangi uzaklık(lar) toplamı ile yerleştiği tutuluyor..
                }
            }
            return seatLayout;
        }

        private static void PrintSeatLayout(Passenger[,] seatLayout)
        {
            Console.WriteLine("|-----------------------OTOBÜS DÜZENİ(Yolcu No - Yolcu İsim)-----------------------|\n" +
                              "|----------------------------------------------------------------------------------|");
            for (int i = 0; i < seatLayout.GetLength(0); i++)
            {
                for (int j = 0; j < seatLayout.GetLength(1); j++)
                {
                    if (j == 2) Console.Write("    ");

                    Console.Write("| " + seatLayout[i, j].Index.ToString("00") + " - " + seatLayout[i, j].Name + " |");
                }

                Console.WriteLine("\n|----------------------------------------------------------------------------------|");
            }
            Console.WriteLine("\n");
        }

        private static double PrintSeatLayoutWithTotalDistance(Passenger[,] seatLayout)
        {
            double counter = 0;
            Console.WriteLine("|-----YOLCULARIN TOPLAM UZAKLIKLARI----|\n" +
                              "|--------------------------------------|");
            for (int i = 0; i < seatLayout.GetLength(0); i++)
            {
                for (int j = 0; j < seatLayout.GetLength(1); j++)
                {
                    if (j == 2) Console.Write("    ");

                    Console.Write("| " + seatLayout[i, j].SumDistance.ToString("00.00") + " |");
                    counter += seatLayout[i, j].SumDistance;
                }
                Console.WriteLine("\n|--------------------------------------|");
            }
            Console.WriteLine("\n");
            return counter;
        }
    }
}