# AI Powered Appointment Booking with MAUI Scheduler

This sample demonstrates how to book an appointment using an AI service in the .NET MAUI Scheduler control within a .NET MAUI application.

## Sample

```xaml
    <scheduler:SfScheduler x:Name="scheduler" View="TimelineDay"
                           AppointmentsSource="{Binding appointments}">
        <scheduler:SfScheduler.TimelineView>
            <scheduler:SchedulerTimelineView StartHour="9" TimeInterval="0:30:0" 
                                             TimeIntervalWidth="90" TimeFormat="hh:mm" 
                                             EndHour="18" />
        </scheduler:SfScheduler.TimelineView>

        <scheduler:SfScheduler.ResourceView>
            <scheduler:SchedulerResourceView Resources="{Binding Resources}">
                <scheduler:SchedulerResourceView.Mapping>
                    <scheduler:SchedulerResourceMapping Name="Name" Id="Id"
                                                        Background="Background"
                                                        Foreground="Foreground" />
                </scheduler:SchedulerResourceView.Mapping>

                <scheduler:SchedulerResourceView.HeaderTemplate>
                    <DataTemplate>
                        <StackLayout Padding="5" Orientation="Vertical"
                                     VerticalOptions="Center" HorizontalOptions="Fill">
                            <Grid>
                                <Border StrokeThickness="2" Background="{Binding Background}"
                                        HorizontalOptions="Center" HeightRequest="65"
                                        WidthRequest="65">
                                    <Border.StrokeShape>
                                        <RoundRectangle CornerRadius="150" />
                                    </Border.StrokeShape>
                                </Border>

                                <Image WidthRequest="50" HeightRequest="50" Aspect="Fill" 
                                       HorizontalOptions="Center" VerticalOptions="Center"
                                       Source="{Binding DataItem.ImageName, Converter={StaticResource imageConverter}}"/>
                            </Grid>

                            <Label Text="{Binding Name}" FontSize="10"
                                   VerticalTextAlignment="Center"
                                   HorizontalTextAlignment="Center" />
                        </StackLayout>
                    </DataTemplate>
                </scheduler:SchedulerResourceView.HeaderTemplate>
            </scheduler:SchedulerResourceView>
        </scheduler:SfScheduler.ResourceView>
    </scheduler:SfScheduler>
```

## Requirements to run the demo

To run the demo, refer to [System Requirements for .NET MAUI](https://help.syncfusion.com/maui/system-requirements)

## Troubleshooting:

### Path too long exception

If you are facing path too long exception when building this example project, close Visual Studio and rename the repository to short and build the project.

## License

Syncfusion has no liability for any damage or consequence that may arise from using or viewing the samples. The samples are for demonstrative purposes. If you choose to use or access the samples, you agree to not hold Syncfusion liable, in any form, for any damage related to use, for accessing, or viewing the samples. By accessing, viewing, or seeing the samples, you acknowledge and agree Syncfusion's samples will not allow you seek injunctive relief in any form for any claim related to the sample. If you do not agree to this, do not view, access, utilize, or otherwise do anything with Syncfusion's samples.
