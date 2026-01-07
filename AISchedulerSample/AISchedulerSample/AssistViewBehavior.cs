using Syncfusion.Maui.AIAssistView;
using Syncfusion.Maui.Scheduler;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
namespace AISchedulerSample
{ 
    public class AssistViewBehavior : Behavior<SfAIAssistView>
    {
        private SfAIAssistView? assistView;
        public SfScheduler? Scheduler { get; set; }
        public SchedulerViewModel? SchedulerViewModel { get; set; }

        protected override void OnAttachedTo(SfAIAssistView bindable)
        {
            base.OnAttachedTo(bindable);
            this.assistView = bindable;
            if (assistView != null)
            {
                assistView.Request += OnAssistViewRequest;
            }
        }

        protected override void OnDetachingFrom(SfAIAssistView bindable)
        {
            base.OnDetachingFrom(bindable);
            this.assistView!.Request -= OnAssistViewRequest;
        }

        private async void OnAssistViewRequest(object? sender, RequestEventArgs e)
        {
            this.SchedulerViewModel!.ShowHeader = false;
            string requeststring = e.RequestItem!.Text;
            DateTime sophiaStartTime;
            DateTime sophiaEndTime;
            string sophiaSubject = string.Empty;
            string sophiaLocation = string.Empty;
            string sophiaResourceID = string.Empty;
            DateTime johnStartTime;
            DateTime johnEndTime;
            string johnSubject = string.Empty;
            string johnLocation = string.Empty;
            string johnResourceID = string.Empty;

            if (string.IsNullOrEmpty(e.RequestItem!.Text))
            {
                return;
            }

            string pattern = @"\b\d{2}:\d{2} (AM|PM)\b";
            bool isValidPattern = Regex.IsMatch(requeststring, pattern);
            if (!isValidPattern)
            {
                await SchedulerViewModel!.GetAIResults(e.RequestItem.Text).ConfigureAwait(true);
            }
            else
            {
                if(requeststring.StartsWith("John: ", StringComparison.OrdinalIgnoreCase))
                {
                    requeststring = requeststring.Substring("John: ".Length);
                }
                else if(requeststring.StartsWith("Sophia: ", StringComparison.OrdinalIgnoreCase))
                {
                    requeststring = requeststring.Substring("Sophia: ".Length);
                }

                for(int i = 0; i < SchedulerViewModel!.SophiaAvailableTimeSlots?.Count; i++)
                {
                    if(requeststring == SchedulerViewModel.SophiaAvailableTimeSlots[i].ToString())
                    {
                        sophiaStartTime = SchedulerViewModel.SophiaStartTimeCollection![i];
                        sophiaEndTime = SchedulerViewModel.SophiaEndTimeCollection![i];
                        sophiaSubject = SchedulerViewModel.SophiaSubjectCollection![i];
                        sophiaLocation = SchedulerViewModel.SophiaLocationCollection![i];
                        sophiaResourceID = SchedulerViewModel.SophiaResourceIDCollection![i];
                        AppointmentBooking(sophiaStartTime, sophiaEndTime, sophiaSubject, sophiaLocation, sophiaResourceID);
                        await Task.Delay(1000);
                        AssistItem botMessage = new AssistItem()
                        {
                            Text = "Doctor Sophia appointment successfully booked. \nThank you!",
                            ShowAssistItemFooter = false
                        };
                        SchedulerViewModel.Messages.Add(botMessage);
                    }
                    else
                    {
                        continue;
                    }
                }

                for (int j = 0; j < SchedulerViewModel.JohnAvailableTimeSlots?.Count; j++)
                {
                    if (requeststring == SchedulerViewModel.JohnAvailableTimeSlots[j].ToString())
                    {
                        johnStartTime = SchedulerViewModel.JohnStartTimeCollection![j];
                        johnEndTime = SchedulerViewModel.JohnEndTimeCollection![j];
                        johnSubject = SchedulerViewModel.JohnSubjectCollection![j];
                        johnLocation = SchedulerViewModel.JohnLocationCollection![j];
                        johnResourceID = SchedulerViewModel.JohnResourceIDCollection![j];
                        AppointmentBooking(johnStartTime, johnEndTime, johnSubject, johnLocation, johnResourceID);
                        await Task.Delay(1000);
                        AssistItem botMessage = new AssistItem() { Text = "Doctor John appointment successfully booked.\nThank you! ", ShowAssistItemFooter = false };
                        SchedulerViewModel.Messages.Add(botMessage);
                    }
                }
            }
        }

        private void AppointmentBooking(DateTime startTime, DateTime endTime, string subject, string location, string resourceID)
        {
            Scheduler!.DisplayDate = startTime;
            SchedulerViewModel?.Appointments?.Add(new SchedulerAppointment()
            {
                StartTime = startTime,
                EndTime = endTime,
                Subject = subject,
                Location = location,
                ResourceIds = new ObservableCollection<object> { resourceID },
                Background = resourceID == "1000" ? new SolidColorBrush(Color.FromArgb("#36B37B")) : new SolidColorBrush(Color.FromArgb("#8B1FA9"))
            });

            (Scheduler!.AppointmentsSource as ObservableCollection<SchedulerAppointment>)?.AddRange(SchedulerViewModel?.Appointments!);
        }
    }

    internal static class Utils
    {
        public static void AddRange<T>(this ObservableCollection<T> collection,  IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
}
