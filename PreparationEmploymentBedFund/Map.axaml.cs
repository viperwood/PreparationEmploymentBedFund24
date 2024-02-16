using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json;

namespace PreparationEmploymentBedFund;

public partial class Map : Window
{
    public Map()
    {
        InitializeComponent();
        FreeBed();
    }

    private List<Rooms> roomsList = new();
    private List<PatientsBed> PatientsBedLists = new();
    private List<Beds> BedsList = new();

    private async Task FreeBed()
    {
        using (var client = new HttpClient())
        {
            HttpResponseMessage responseMessage = await client.GetAsync("http://localhost:5263/api/Room/GetRoom");
            string content = await responseMessage.Content.ReadAsStringAsync();
            roomsList = JsonConvert.DeserializeObject<List<Rooms>>(content)!.ToList();
            RoomBox.ItemsSource = roomsList.Select(x => new
            {
                RoomName = x.Roomname
            }).ToList();
            
            HttpResponseMessage responseMes = await client.GetAsync("http://localhost:5263/api/Patients1/GetPatientsFromBed");
            string contents = await responseMes.Content.ReadAsStringAsync();
            PatientsBedLists = JsonConvert.DeserializeObject<List<PatientsBed>>(contents)!.ToList();
            PatientBox.ItemsSource = PatientsBedLists.Select(x => new
            {
                PatientName = x.Patientsname
            }).ToList();
            
            HttpResponseMessage Message = await client.GetAsync("http://localhost:5263/api/Bed/GetBed");
            string cont = await Message.Content.ReadAsStringAsync();
            BedsList = JsonConvert.DeserializeObject<List<Beds>>(cont)!.ToList();
            BedBox.ItemsSource = BedsList.Select(x => new
            {
                BedName = x.Bedname
            }).ToList();
        }
    }

    private async void Save(object? sender, RoutedEventArgs e)
    {
        AddBedPatient addBedPatient = new AddBedPatient();
        string namebedIndex = BedsList[BedBox.SelectedIndex].Bedname.ToList()[0].ToString();
        addBedPatient.Bedid = BedsList.Where(x => x.Bedname == namebedIndex).Select(x => x.Idbed).ToList()[0];
        addBedPatient.Patientsid = PatientsBedLists
            .Where(x => x.Patientsname == PatientsBedLists[PatientBox.SelectedIndex].Patientsname)
            .Select(x => x.Idpatients).ToList()[0];
        addBedPatient.Idroom = roomsList.Where(x => x.Roomname == roomsList[RoomBox.SelectedIndex].Roomname)
            .Select(x => x.Idroom).ToList()[0];
        using (var client = new HttpClient())
        {
            HttpResponseMessage responseMessage = await client.PostAsJsonAsync("http://localhost:5263/api/BedOccupancy/PostRegBed", addBedPatient);
        }
    }
}

public class AddBedPatient
{   
    public int? Idroom { get; set; }
    public int? Bedid { get; set; }
    public int? Patientsid { get; set; }
}

class Rooms
{
    public int? Idroom { get; set; }

    public string Roomname { get; set; }
}

class Beds
{
    public int? Idbed { get; set; }

    public string Bedname { get; set; }
}

class PatientsBed
{
    public int? Idpatients { get; set; }

    public string Patientsname { get; set; }
}