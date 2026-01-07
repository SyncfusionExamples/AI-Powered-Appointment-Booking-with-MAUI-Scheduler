using Newtonsoft.Json.Linq;
using Syncfusion.Maui.AIAssistView;
using Syncfusion.Maui.Scheduler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace AISchedulerSample
{
    public partial class MainPage : ContentPage
    {
        private SfAIAssistView? sfAIAssistView;
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            sfAIAssistView = (SfAIAssistView?)FindByName("aiAssistView");
            headerView = (Border?)FindByName("headerView");
        }

        private void aibutton_Clicked(object sender, EventArgs e)
        {
            if (sfAIAssistView != null && headerView != null)
            {
                bool isVisible = !sfAIAssistView.IsVisible;
                sfAIAssistView.IsVisible = isVisible;
                headerView.IsVisible = isVisible;
            }
        }
    }

    public class Model
    {
        public string? Name { get; set; }
        public string? Id { get; set; }
        public Brush? Background { get; set; }
        public Brush? Foreground { get; set; }
        public string? ImageName { get; set; }
    }

    public class AppointmentModel
    {
        public AppointmentModel()
        {
            Name = string.Empty;
            ImageName = string.Empty;
        }

        public string Name { get; set; }
        public string ImageName { get; set; }
    }

    public class SchedulerViewModel : INotifyPropertyChanged
    {
        private string returnMessage = string.Empty;

        private ObservableCollection<IAssistItem> messages;

        private bool showHeader = true;

        private AzureOpenAIService azureAIServices = new AzureOpenAIService();

        private bool showAssistView = false;

        internal List<DateTime>? SophiaStartTimeCollection;

        internal List<DateTime>? SophiaEndTimeCollection;

        internal List<string>? SophiaSubjectCollection;

        internal List<string>? SophiaLocationCollection;

        internal List<string>? SophiaResourceIDCollection;

        internal List<DateTime>? JohnStartTimeCollection;

        internal List<DateTime>? JohnEndTimeCollection;

        internal List<string>? JohnSubjectCollection;

        internal List<string>? JohnLocationCollection;

        internal List<string>? JohnResourceIDCollection;

        internal List<string>? SophiaAvailableTimeSlots = new List<string>();

        internal List<string>? JohnAvailableTimeSlots = new List<string>();

        public ObservableCollection<SchedulerAppointment>? Appointments = new ObservableCollection<SchedulerAppointment>();

        public ObservableCollection<IAssistItem> Messages
        {
            get
            {
                return this.messages;
            }

            set
            {
                this.messages = value;
            }
        }

        public bool ShowHeader
        {
            get { return this.showHeader; }
            set { this.showHeader = value; RaisePropertyChanged("ShowHeader"); }
        }

        public bool ShowAssistView
        {
            get { return this.showAssistView; }
            set { this.showAssistView = value; RaisePropertyChanged("ShowAssistView"); }
        }

        private ObservableCollection<object>? resources;
        public ObservableCollection<object>? Resources
        {
            get
            {
                return this.resources;
            }

            set
            {
                this.resources = value;
            }
        }

        public ObservableCollection<SchedulerAppointment> appointments { get; set; }
        public SchedulerViewModel()
        {
            messages = new ObservableCollection<IAssistItem>();
            Resources = new ObservableCollection<object>();
            InitializeResources();
            InitialAppointmentBooking();
        }

        private void InitialAppointmentBooking()
        {
            appointments = new ObservableCollection<SchedulerAppointment>()
            {
                new SchedulerAppointment()
                {
                    StartTime = DateTime.Today.AddHours(15),
                    EndTime = DateTime.Today.AddHours(15).AddMinutes(30),
                    Subject = "General Check-Up",
                    Location = "ABC hospital",
                    Background = new SolidColorBrush(Color.FromArgb("#36B37B")),
                    ResourceIds = new ObservableCollection<object>() { "1000" }
                },
                new SchedulerAppointment()
                {
                    StartTime = DateTime.Today.AddHours(10),
                    EndTime = DateTime.Today.AddHours(10).AddMinutes(30),
                    Subject = "Vaccinations",
                    Location = "ABC hospital",
                    Background = new SolidColorBrush(Color.FromArgb("#36B37B")),
                    ResourceIds = new ObservableCollection<object>() { "1000" }
                },
                new SchedulerAppointment()
                {
                    StartTime = DateTime.Today.AddHours(9),
                    EndTime = DateTime.Today.AddHours(9).AddMinutes(30),
                    Subject = "Diagnostic report",
                    Location = "ABC hospital",
                    Background = new SolidColorBrush(Color.FromArgb("#8B1FA9")),
                    ResourceIds = new ObservableCollection<object>() { "1001" }
                },
                new SchedulerAppointment()
                {
                    StartTime = DateTime.Today.AddHours(16),
                    EndTime = DateTime.Today.AddHours(16).AddMinutes(30),
                    Subject = "Diabetes",
                    Location = "ABC hospital",
                    Background = new SolidColorBrush(Color.FromArgb("#8B1FA9")),
                    ResourceIds = new ObservableCollection<object>() { "1001" }
                }
            };
        }

        private void InitializeResources()
        {
            for (int i = 0; i < 2; i++)
            {
                Model resourceViewModel = new Model();
                if (i == 0)
                {
                    resourceViewModel.Name = "Sophia";
                    resourceViewModel.ImageName = "sophia.png";
                    resourceViewModel.Id = "1000";
                    resourceViewModel.Background = new SolidColorBrush(Color.FromArgb("#36B37B"));
                }
                else
                {
                    resourceViewModel.Name = "John";
                    resourceViewModel.ImageName = "john.png";
                    resourceViewModel.Id = "1001";
                    resourceViewModel.Background = new SolidColorBrush(Color.FromArgb("#8B1FA9"));
                }

                Resources?.Add(resourceViewModel);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public async Task GetAIResults(string query)
        {
            await Task.Delay(1000).ConfigureAwait(true);
            var reply = await GetRecommendation(query);
            var chatSuggestions = new AssistItemSuggestion();
            var suggestions = new ObservableCollection<ISuggestion>();

            if (query.Contains("Sophia", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var timeSlot in SophiaAvailableTimeSlots!)
                {
                    suggestions.Add(new AssistSuggestion() { Text = timeSlot });
                }
            }
            else if (query.Contains("John", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var timeSlot in JohnAvailableTimeSlots!)
                {
                    suggestions.Add(new AssistSuggestion() { Text = timeSlot });
                }
            }
            else
            {
                foreach (var timeSlot in SophiaAvailableTimeSlots!)
                {
                    suggestions.Add(new AssistSuggestion() { Text = "Sophia: " + timeSlot });
                }

                foreach (var timeSlot in JohnAvailableTimeSlots!)
                {
                    suggestions.Add(new AssistSuggestion() { Text = "John: " + timeSlot });
                }
            }

            chatSuggestions.Items = suggestions;
            chatSuggestions.Orientation = SuggestionsOrientation.Vertical;

            AssistItem botMessage = new AssistItem()
            {
                Text = reply,
                Suggestion = chatSuggestions,
                ShowAssistItemFooter = false
            };

            Messages.Add(botMessage);
        }

        private async Task<string> GetRecommendation(string userInput)
        {
            DateTime todayDate = DateTime.Today;
            string prompt = $"Given data: {userInput}. Based on the given data, provide 10 appointment time details for Doctor1 and Doctor2 on {todayDate}." +
                            $"Availability time is 9AM to 6PM." +
                            $"In 10 appointments, split the time details as 5 for Doctor1 and 5 for Doctor2." +
                            $"Provide complete appointment time details for both Doctor1 and Doctor2 without missing any fields." +
                            $"It should be 30 minutes appointment duration." +
                            $"Doctor1 time details should not collide with Doctor2." +
                            $"Provide ResourceID for Doctor1 as 1000 and for Doctor2 as 1001." +
                            $"Do not repeat the same time. Generate the following fields: StartDate, EndDate, Subject, Location, and ResourceID." +
                            $"The return format should be the following JSON format: Doctor1[StartDate, EndDate, Subject, Location, ResourceID], Doctor2[StartDate, EndDate, Subject, Location, ResourceID]." +
                            $"Condition: provide details without any explanation. Don't include any special characters like ```";

            returnMessage = await azureAIServices.GetResponseFromGPT(prompt);
            returnMessage = returnMessage.Replace("```json", "").Replace("```", "").Trim();
            returnMessage = System.Text.RegularExpressions.Regex.Replace(returnMessage, @"(\d{4}-\d{2}-\d{2})T(\d{2}:\d{2}:\d{2})", "$1 $2");
            var jsonObj = JObject.Parse(returnMessage);

            var doctorAppointments = new Dictionary<string, (List<DateTime> StartTimes, List<DateTime> EndTimes, List<string> Subjects, List<string> Locations, List<string> ResourceIDs)>
            {
                { "Doctor1", (new List<DateTime>(), new List<DateTime>(), new List<string>(), new List<string>(), new List<string>()) },
                { "Doctor2", (new List<DateTime>(), new List<DateTime>(), new List<string>(), new List<string>(), new List<string>()) }
            };

            foreach (var doctor in doctorAppointments.Keys)
            {
                foreach (var appointment in jsonObj[doctor]!)
                {
                    if (DateTime.TryParse((string)appointment["StartDate"]!, out DateTime startTime) && DateTime.TryParse((string)appointment["EndDate"]!, out DateTime endTime))
                    {
                        doctorAppointments[doctor].StartTimes.Add(startTime);
                        doctorAppointments[doctor].EndTimes.Add(endTime);
                    }

                    doctorAppointments[doctor].Subjects.Add((string)appointment["Subject"]!);
                    doctorAppointments[doctor].Locations.Add((string)appointment["Locations"]!);
                    doctorAppointments[doctor].ResourceIDs.Add((string)appointment["ResourceID"]!);
                }
            }

            SophiaStartTimeCollection = doctorAppointments["Doctor1"].StartTimes;
            SophiaEndTimeCollection = doctorAppointments["Doctor1"].EndTimes;
            SophiaSubjectCollection = doctorAppointments["Doctor1"].Subjects;
            SophiaLocationCollection = doctorAppointments["Doctor1"].Locations;
            SophiaResourceIDCollection = doctorAppointments["Doctor1"].ResourceIDs;

            JohnStartTimeCollection = doctorAppointments["Doctor2"].StartTimes;
            JohnEndTimeCollection = doctorAppointments["Doctor2"].EndTimes;
            JohnSubjectCollection = doctorAppointments["Doctor2"].Subjects;
            JohnLocationCollection = doctorAppointments["Doctor2"].Locations;
            JohnResourceIDCollection = doctorAppointments["Doctor2"].ResourceIDs;

            FilterAppointmentsForSophia();
            FilterAppointmentsForJohn();

            SophiaAvailableTimeSlots = GenerateTimeSlots(SophiaStartTimeCollection);
            JohnAvailableTimeSlots = GenerateTimeSlots(JohnStartTimeCollection);

            return GenerateFinalTimeSlots(userInput);
        }

        private string GenerateFinalTimeSlots(string userInput)
        {
            if (userInput.Contains("Sophia", StringComparison.OrdinalIgnoreCase))
            {
                return "Doctor Sophia available appointment slots:";
            }
            else if (userInput.Contains("John", StringComparison.OrdinalIgnoreCase))
            {
                return "Doctor John available appointment slots:";
            }
            else
            {
                return "Only Doctors Sophia and John are available, and their appointment slots:";
            }
        }

        private List<string> GenerateTimeSlots(List<DateTime> timeCollection)
        {
            return timeCollection.Select(time => time.ToString("hh:mm tt").ToUpper()).ToList();
        }

        private void FilterAppointmentsForJohn()
        {
            for (int i = 0; i < Appointments?.Count; i++)
            {
                if (JohnStartTimeCollection!.Contains(Appointments[i].StartTime))
                {
                    JohnStartTimeCollection.Remove(Appointments[i].StartTime);
                }
            }
        }

        private void FilterAppointmentsForSophia()
        {
            for (int i = 0; i < Appointments?.Count; i++)
            {
                if (SophiaStartTimeCollection!.Contains(Appointments[i].StartTime))
                {
                    SophiaStartTimeCollection.Remove(Appointments[i].StartTime);
                }
            }
        }
    }

    public class SfImageSourceConverter : IValueConverter
    {
        public object? Convert(object? value, Type? targetType, object? parameter, CultureInfo culture)
        {
            string? source = value as string;
            string? assemblyName = typeof(SfImageSourceConverter).GetTypeInfo().Assembly.GetName().Name;
            return ImageSource.FromResource(assemblyName + ".Resources.Images." + source, typeof(SfImageSourceConverter).GetTypeInfo().Assembly);
        }

        public object? ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
