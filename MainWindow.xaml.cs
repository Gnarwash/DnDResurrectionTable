using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;

namespace DNDTisch
{
    public partial class MainWindow : Window
    {
        private List<Entry> entries = new List<Entry>();

        public MainWindow()
        {
            InitializeComponent();
            LoadEntriesFromXml();
            DataGrid.ItemsSource = entries;
        }

        private void CalculateWeightButton_Click(object sender, RoutedEventArgs e)
        {
            int totalWeight = 1000;

            // Filtere Einträge mit Gewichtung
            var entriesWithWeight = entries.Where(entry => entry.Gewichtung > 0).ToList();

            // Berechne den Gesamtgewichtungswert der vorhandenen Einträge
            double totalWeightSum = entriesWithWeight.Sum(entry => entry.Gewichtung);

            // Berechne die Gewichtung für Einträge ohne explizite Gewichtung
            double remainingWeight = totalWeight - totalWeightSum;
            int entriesWithoutWeightCount = entries.Count - entriesWithWeight.Count;
            double weightPerEntry = entriesWithoutWeightCount > 0 ? remainingWeight / entriesWithoutWeightCount : 0;

            // Runde die Gewichtungswerte auf ganze Zahlen
            int roundedWeightPerEntry = (int)Math.Round(weightPerEntry);

            // Aktualisiere die Gewichtungen und Zahlen des 1d1000
            int currentPosition = 1;
            foreach (Entry entry in entries)
            {
                if (entry.Gewichtung == 0)
                {
                    entry.Gewichtung = roundedWeightPerEntry;
                }
                entry.Zahl1d1000 = currentPosition;
                currentPosition += entry.Gewichtung;
            }

            DataGrid.Items.Refresh();

            // Zeige die Gewichtungssumme
            int totalWeightSumRounded = entries.Sum(entry => entry.Gewichtung);
            GewichtungSummeTextBlock.Text = $"Gewichtungssumme: {totalWeightSumRounded}";
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            SaveEntriesToXml();
            MessageBox.Show("Die Einträge wurden erfolgreich als XML exportiert.", "Export abgeschlossen", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SaveEntriesToXml()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));
            using (StreamWriter writer = new StreamWriter("entries.xml"))
            {
                serializer.Serialize(writer, entries);
            }
        }

        private void LoadEntriesFromXml()
        {
            if (File.Exists("entries.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));
                using (StreamReader reader = new StreamReader("entries.xml"))
                {
                    entries = (List<Entry>)serializer.Deserialize(reader);
                }
            }
        }

        
    }
}
