using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Newtonsoft.Json;
using NovinDevHubStaffCore.Models;
using NovinDevHubStaffCore.Services;
using Refit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace NovinDevHubStaffCore.Core.Activities
{
    public class SystemActivity
    {
        const string IsoDateFormat = "yyyy-MM-ddTHH:mm:ssZ";
        private readonly IApplicationService applicationService = Ioc.Default.GetRequiredService<IApplicationService>();
        private readonly IActivityService activityService = Ioc.Default.GetRequiredService<IActivityService>();
        private readonly IUrlService urlService = Ioc.Default.GetRequiredService<IUrlService>();
        private readonly IScreenshotService screenshotService = Ioc.Default.GetRequiredService<IScreenshotService>();
        private readonly IFileService fileService = Ioc.Default.GetRequiredService<IFileService>();

        public SystemActivity(Task Task)
        {
            Id = Guid.NewGuid().ToString();

            this.Task = Task;

            ScreenCaptures = new List<ScreenCapture>();
            ProccessActivities = new List<ProccessActivity>();
            BrowserActivities = new List<BrowserActivity>();
            MouseKeyboardActivities = new List<MouseKeyboardActivity>();

            StartTime = DateTime.Now;

        }
        public string Id { get; set; }

        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

        public Task Task { get; set; }
        public List<ScreenCapture> ScreenCaptures { get; set; }
        public List<ProccessActivity> ProccessActivities { get; set; }
        public List<BrowserActivity> BrowserActivities { get; set; }
        public List<MouseKeyboardActivity> MouseKeyboardActivities { get; set; }

        public TimeSpan TotalDurationSeconds { get; set; }
        public void IncreaseTime()
        {
            TotalDurationSeconds = TotalDurationSeconds.Add(TimeSpan.FromSeconds(1));
        }

        public void Stop()
        {
            EndTime = DateTime.Now;
            SyncData();
        }

        private async void SyncData()
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                SyncApplications();
                SyncBrowserActivities();
                SyncActivities();
                SyncScreenshots();
            });

        }

        private async void SyncActivities()
        {
            var xmlDataSource = new XmlDataSource();
            var keyboardActivities = MouseKeyboardActivities.Where(x => x.Type == 1).ToList();
            var mouseActivities = MouseKeyboardActivities.Where(x => x.Type == 2).ToList();

            var keyboardTime = (int)keyboardActivities.Sum(x => (x.EndActiveTime - x.StartActiveTime).TotalSeconds);
            var mouseTime = (int)mouseActivities.Sum(x => (x.EndActiveTime - x.StartActiveTime).TotalSeconds);
            var sumOverlap = 0;
            var totalTime = (EndTime - StartTime).TotalSeconds;

            foreach (var k in keyboardActivities)
                foreach (var m in mouseActivities)
                    sumOverlap += (int)k.OverlapPeriod(m).TotalSeconds;

            var activity = new Models.Activity
            {
                ProjectId = this.Task.Project.Id,
                TaskId = this.Task.Id,
                StartTime = StartTime.ToString(IsoDateFormat),
                Tracked = (int)totalTime,
                Keyboard = (int)((keyboardTime / totalTime) * 100),
                Mouse = (int)((mouseTime / totalTime) * 100),
                Overall = sumOverlap,

            };
            try
            {
                var response = await activityService.Post(activity);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content;
                    Debug.WriteLine("MouseKeyboard Activity Saved: " + result.Data.Id);
                }
                else//Save to xml file to sync later
                {
                    Debug.WriteLine("MouseKeyboard sync was unsuccessfull, writing to file ");
                    xmlDataSource.MouseKeyboardActivies.Add(activity);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MouseKeyboard Activity save Error: " + ex.Message);
                xmlDataSource.MouseKeyboardActivies.Add(activity);
            }


            //Save all unsuccessfull data to xml file
            xmlDataSource.SaveActivity();
        }

        private async void SyncApplications()
        {
            var xmlDataSource = new XmlDataSource();
            foreach (var p in ProccessActivities)
            {
                //if it was last current activity 
                if (p.EndTime == null)
                    p.EndTime = DateTime.Now;
                var app = new Models.Application
                {
                    Name = p.ProcessName + " - " + p.WindowTitle,
                    ProjectId = this.Task.Project.Id,
                    TaskId = this.Task.Id,
                    Title = p.WindowTitle,
                    StartTime = p.StartTime.ToString(IsoDateFormat),
                    Tracked = (int)(p.EndTime.Value - p.StartTime).TotalSeconds
                };
                try
                {
                    var response = await applicationService.Post(app);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content;
                        Debug.WriteLine("Activity Saved: " + result.Data.Id);
                    }
                    else//Save to xml file to sync later
                    {
                        Debug.WriteLine("Activity sync was unsuccessfull, writing to file ");
                        xmlDataSource.ProccessUsages.Add(p);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(" Activity save Error: " + ex.Message);
                    xmlDataSource.ProccessUsages.Add(p);
                }
            }
            //Save all unsuccessfull data to xml file
            xmlDataSource.SaveApplication();
        }

        private async void SyncBrowserActivities()
        {
            var xmlDataSource = new XmlDataSource();
            foreach (var ba in BrowserActivities)
            {
                //if it was last current activity 
                if (ba.EndTime == null)
                    ba.EndTime = DateTime.Now;
                var url = new Models.UrlActivity
                {
                    Url = ba.Url,
                    ProjectId = this.Task.Project.Id,
                    TaskId = this.Task.Id,
                    FromTime = ba.StartTime.ToString(IsoDateFormat),
                    ToTime = ba.EndTime.Value.ToString(IsoDateFormat),
                    Title = ba.BrowserName
                };

                try
                {
                    var response = await urlService.Post(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content;
                        Debug.WriteLine("Browser Saved: " + result.Data.Id);
                    }
                    else//Save to xml file to sync later
                    {
                        Debug.WriteLine("Browser sync was unsuccessfull, writing to file ");
                        xmlDataSource.BrowsedUrls.Add(ba);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Browser save Error: " + ex.Message);
                    xmlDataSource.BrowsedUrls.Add(ba);
                }
            }
            //Save all unsuccessfull data to xml file
            xmlDataSource.SaveBrowsedUrls();
        }

        private async void SyncScreenshots()
        {
            var xmlDataSource = new XmlDataSource();
            foreach (var sc in ScreenCaptures)
            {
                var image = new StreamPart(File.OpenRead(sc.ImageFilePath), Path.GetFileName(sc.ImageFilePath), "image/jpeg");
                var result = await fileService.UploadPictureAsync(image);
                if (result.IsSuccessStatusCode)
                {
                    var thumbnail = new StreamPart(File.OpenRead(sc.ImageFilePath), Path.GetFileName(sc.ThumbnailFilePath), "image/jpeg");
                    var resultThumb = await fileService.UploadPictureAsync(thumbnail);

                    if (resultThumb.IsSuccessStatusCode)
                    {
                        var resultScreenshot = await screenshotService.Screenshot(new ScreenshotData
                        {
                            CreateAt = sc.CaptureTime.ToString(IsoDateFormat),
                            ThumbUrl = resultThumb.Content.Data,
                            Url = result.Content.Data
                        });
                        if (!resultScreenshot.IsSuccessStatusCode)
                        {
                            Debug.WriteLine("Screenshot sync was unsuccessfull, writing to file ");
                            xmlDataSource.Screenshots.Add(sc);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Uploading thumbnail was unsuccessfull, writing to file ");
                        xmlDataSource.Screenshots.Add(sc);
                    }
                }
                else
                {
                    Debug.WriteLine("Uploading screenshot was unsuccessfull, writing to file ");
                    xmlDataSource.Screenshots.Add(sc);
                }
            }
            //Save all unsuccessfull data to xml file
            xmlDataSource.SaveScreenshots();
        }
    }
}
