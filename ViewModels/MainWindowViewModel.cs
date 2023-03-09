using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using NovinDevHubStaffCore.Core;
using NovinDevHubStaffCore.Core.Activities;
using NovinDevHubStaffCore.Models;
using NovinDevHubStaffCore.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace NovinDevHubStaffCore.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        private readonly IAuthTokenStore authTokenStore = Ioc.Default.GetRequiredService<IAuthTokenStore>();
        private readonly IProjectService projectService = Ioc.Default.GetRequiredService<IProjectService>();
        private readonly ITaskService taskService = Ioc.Default.GetRequiredService<ITaskService>();

        const string chrome = "chrome";
        const string firefox = "firefox";
        const int min10 = 10 * 60;
        const int min2 = 2 * 60;

        SystemActivity CurrentActivity;
        ProccessActivity lastProccessActivity;
        BrowserActivity lastBrowserActivity;

        SystemActivityMonitor monitor;
        DispatcherTimer activityTimer;
        IExpandable expandable;

        public MainWindowViewModel(IExpandable container)
        {
            LoginCommand = new RelayCommand(LoginHandler);
            LogoutCommand = new RelayCommand(LogoutHandler);
            StartStopCommand = new RelayCommand(StartStopHandler);
            CompleteTaskCommand = new RelayCommand(CompleteTaskHandler);
            SelectProjectCommand = new RelayCommand<Project>(SelectProjectHandler);
            StartStopTaskCommand = new RelayCommand<Models.Task>(StartStopTaskHandler);
            SelectedTaskChangedCommand = new RelayCommand<Models.Task>(SelectedTaskChangedHandler);
            expandable = container;

            monitor = new SystemActivityMonitor();
           // monitor.OnProccessChanged += Monitor_OnProccessChanged;
           // monitor.OnKeyboardMouseActivity += Monitor_OnKeyboardMouseActivity;

            activityTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            activityTimer.Tick += OnTimerTick;
            TotalDurationSeconds = TimeSpan.FromSeconds(0);

           // Init();
           
        }

        private async void Init()
        {
            var login = await authTokenStore.GetToken();
            if (login != null)
            {
                IsLoggedIn = true;
                await InitData();
            }
              
        }



        #region Dependecy Properties

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set => SetProperty(ref _isLoggedIn, value);
        }

        private bool _isStarted;
        public bool IsStarted
        {
            get => _isStarted;
            set => SetProperty(ref _isStarted, value);
        }

        private string _status;
        public string Status
        {
            get => _status;
            set=>SetProperty(ref _status, value);
        }
        private bool _showAllTasks;
        public bool ShowAllTasks
        {
            get => _showAllTasks;
            set {
                SetProperty(ref _showAllTasks, value);
                LoadProjectTasks();
            }
        }


        private string _searchTaskTerm;
        public string SearchTaskTerm
        {
            get => _searchTaskTerm;
            set
            {
                SetProperty(ref _searchTaskTerm, value);
              
                LoadProjectTasks();
            }
        }

        private string _searchProjectTerm;
        public string SearchProjectTerm
        {
            get => _searchProjectTerm;
            set
            {
                SetProperty(ref _searchProjectTerm, value);
                if (_originalProjects == null)
                    return;
                if (!string.IsNullOrEmpty(_searchProjectTerm))
                    Projects = _originalProjects
                        .Where(x => x.Title.ToLower().Contains(_searchProjectTerm.ToLower()))
                        .ToList();
                else
                    Projects = _originalProjects;
            }
        }

        private List<Project> _originalProjects;
        private List<Project> _projects;
        public List<Project> Projects
        {
            get => _projects;
            set => SetProperty(ref _projects, value);
        }

        private List<Models.Task> _tasks;
        public List<Models.Task> Tasks
        {
            get => _tasks;
            set => SetProperty(ref _tasks, value);
        }

        private Project _selectedProject;
        public Project SelectedProject
        {
            get => _selectedProject;
            set => SetProperty(ref _selectedProject, value);
        }


        private Models.Task _selectedTask;
        public Models.Task SelectedTask
        {
            get => _selectedTask;
            set => SetProperty(ref _selectedTask, value);
        }

        private Models.Task _activeTask;
        public Models.Task ActiveTask
        {
            get => _activeTask;
            set => SetProperty(ref _activeTask, value);
        }

        private TimeSpan _totalDurationSeconds;
        public TimeSpan TotalDurationSeconds
        {
            get => _totalDurationSeconds;
            set => SetProperty(ref _totalDurationSeconds, value);
        }

        #endregion

        #region Commands

        public ICommand LoginCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand StartStopCommand { get; }
        public ICommand CompleteTaskCommand { get; }
        public IRelayCommand<Project> SelectProjectCommand { get; }
        public IRelayCommand<Models.Task> StartStopTaskCommand { get; }
        public IRelayCommand<Models.Task> SelectedTaskChangedCommand { get; }

        #endregion

        #region Command Handlers

        public void LoginHandler()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "data", "Activity_0");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            //var screenShots = Core.Activities.ScreenCapture.CaptureNative(path);

            LoginDialog inputDialog = new LoginDialog();
            if (inputDialog.ShowDialog() == true)
            {
                IsLoggedIn = true;
                _ = InitData();
            }
        }
        public void LogoutHandler()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "data", "Activity_0" );
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //var screenShots =  Core.Activities.ScreenCapture.CaptureNative(path);

            if (IsStarted)
            {
                MessageBox.Show("Can not logout while task is inprogress");
                return;
            }
            authTokenStore.DeleteToken();
            IsLoggedIn = false;
        }

        public void StartStopHandler()
        {
            if (IsStarted)
                StopTask(ActiveTask);
            else
            {
                if (SelectedTask == null)
                    MessageBox.Show("Please select a task to start");
                else
                    StartTask(SelectedTask);
            }

        }
        public void StartStopTaskHandler(Models.Task task)
        {
            if (ActiveTask != null)
                StopTask(task);
            else
                StartTask(task);

        }

        public void SelectProjectHandler(Project project)
        {
            if (SelectedProject != null)
                SelectedProject.IsSelected = false;
            if (project != null)
                project.IsSelected = true;
               

            SelectedProject = project;
            LoadProjectTasks();
            expandable.Expand();
        }
        public void SelectedTaskChangedHandler(Models.Task task)
        {
            if (SelectedTask != null)
                SelectedTask.IsSelected = false;
            if (task != null)
                task.IsSelected = true;

            SelectedTask = task;

        }

        public async void CompleteTaskHandler()
        {
            IsLoading = true;
            Status = "Completing task...";
            var task_response = await taskService.Complete(new Models.Api.TaskDone { taskId = SelectedTask.Id });
            if (task_response.Content.IsSuccess)
            {
                SelectedTask.isDone = true;
            }
            else
                MessageBox.Show("Server sent error");
            IsLoading = false;
            Status = "Ready";
        }


        #endregion

        #region ------------------private methods-----------------------
        private async System.Threading.Tasks.Task InitData()
        {
            IsLoading = true;
            Status = "Loading projects...";
            var project_response = await projectService.Get();
          
            if (project_response.IsSuccessStatusCode)
            {
                var result = project_response.Content;
                _originalProjects = result.Data.projects;
                Projects = _originalProjects;
            }
            Status = "Loading tasks...";
            var task_response = await taskService.Get();
           
            if (task_response.IsSuccessStatusCode)
            {
                Status = "Finializing...";
                var result = task_response.Content;
                foreach (var p in _originalProjects)
                {
                    p.Tasks = result.Data.tasks.Where(x => x.Project.Id == p.Id).ToList();
                    //Correct task.project reference to project object exist in _originalProjects
                    foreach (var t in p.Tasks)
                        t.Project = p;
                }
                   
            }
            IsLoading = false;
            Status = "Ready";

            return;
            //var projects = new List<Project>();
            //var counter = 1;
            //for (var i = 0; i < 3; i++)
            //{
            //    var project = new Project
            //    {
            //        Id = Guid.NewGuid().ToString(),
            //        Title = $"Project {i}",
            //        IsActive = false,
            //        TotalDurationSeconds = TimeSpan.FromSeconds(0),
            //    };
            //    project.Tasks = new List<Models.Task>
            //            {
            //              new Models.Task{Project= project, Id=Guid.NewGuid().ToString(), Title=$"Task {i*2+counter}",CreatedAt=DateTime.Now},
            //              new Models.Task{Project= project, Id=Guid.NewGuid().ToString(), Title=$"Task  {i*2+counter}",CreatedAt=DateTime.Now}
            //            };
            //    projects.Add(project);
            //    counter++;

            //}
            //_originalProjects = projects;
            //Projects = _originalProjects;
        }
        private void StartTask(Models.Task task)
        {
            Status = "Task is running";
            task.IsActive = true;
            ActiveTask = task;
            IsStarted = true;
            activityTimer.Start();

            //to populat the data for current activity
            CurrentActivity = new SystemActivity(task);
            monitor.Start();

        }
        private void StopTask(Models.Task task)
        {
            if (ActiveTask != task)
            {
                MessageBox.Show("Please stop active task first");
                return;
            }
            monitor.Stop();

            task.IsActive = false;
            IsStarted = false;
            ActiveTask = null;
            activityTimer.Stop();

            CurrentActivity.Stop();
            Status = "Ready";

        }
        private async void OnTimerTick(object sender, EventArgs e)
        {
            TotalDurationSeconds = TotalDurationSeconds.Add(TimeSpan.FromSeconds(1));
            ActiveTask.IncreaseTime();
            ActiveTask.Project.IncreaseTime();
            CurrentActivity.IncreaseTime();

            if (CurrentActivity.TotalDurationSeconds.TotalSeconds % min2 == 0)
            {
                var path = Path.Combine(Environment.CurrentDirectory, "data", "Activity_" + CurrentActivity.Id);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                var screenShots = await System.Threading.Tasks.Task.Run<List<ScreenCapture>>(() =>
                  {
                      return Core.Activities.ScreenCapture.CaptureNative(path);
                  });

                CurrentActivity.ScreenCaptures.AddRange(screenShots);


            }
            if (CurrentActivity.TotalDurationSeconds.TotalSeconds % min10 == 0)
            {
                //Sync data to server asynchronouslly
                CurrentActivity.Stop();
                //Start new Activity
                CurrentActivity = new SystemActivity(ActiveTask);
            }
        }
        private void Monitor_OnProccessChanged(string windowTitle, IntPtr windowHandle, System.Diagnostics.Process process)
        {
            if (ActiveTask != null)
            {
                //If there is prev procosess set endtime of usage
                if (lastProccessActivity != null)
                {
                    lastProccessActivity.EndTime = DateTime.Now;
                    lastProccessActivity.UseDurationSeconds = (int)(lastProccessActivity.EndTime.Value - lastProccessActivity.StartTime).TotalSeconds;
                }
                //If there is prev browser procosess set endtime of browse usage
                if (lastBrowserActivity != null)
                {
                    lastBrowserActivity.EndTime = DateTime.Now;
                    lastBrowserActivity = null;
                }

                lastProccessActivity = new ProccessActivity
                {
                    ProcessId = process.Id,
                    StartTime = DateTime.Now,
                    ProcessName = process.ProcessName,
                    WindowTitle = windowTitle
                };
                CurrentActivity.ProccessActivities.Add(lastProccessActivity);

                //If current process is browser
                if (process.ProcessName == chrome || process.ProcessName == firefox)
                {
                    var url = "";
                    if (process.ProcessName == chrome)
                        url = ApplicationInspector.GetChromeUrlAddress(windowHandle);
                    else
                        url = ApplicationInspector.GetFireFoxUrlAddress(windowHandle);

                    //New window before browse any website cause url be null
                    if (!string.IsNullOrEmpty(url))
                    {
                        lastBrowserActivity = new BrowserActivity
                        {
                            BrowserName = process.ProcessName,
                            StartTime = DateTime.Now,
                            Url = url
                        };
                        CurrentActivity.BrowserActivities.Add(lastBrowserActivity);
                    }
                       
                }

                //End of browser
            }
        }

        private void Monitor_OnKeyboardMouseActivity(short type, DateTime startTime, DateTime endTime)
        {

            if (ActiveTask != null)
            {
                if (type == 1)
                    Debug.WriteLine("Keyboard Activity Created");
                if (type == 2)
                    Debug.WriteLine("Mouse Activity Created");
                CurrentActivity.MouseKeyboardActivities.Add(new MouseKeyboardActivity
                {
                    StartActiveTime = startTime,
                    EndActiveTime = endTime,
                    Type = type
                });
            }
        }
        private void LoadProjectTasks()
        {
            if (_selectedProject == null)
                return;
            if (!string.IsNullOrEmpty(_searchTaskTerm))
                Tasks = _selectedProject.Tasks
                     .Where(x => x.Title.ToLower().Contains(_searchTaskTerm.ToLower()) && (!x.isDone || ShowAllTasks))
                     .ToList();
            else
                Tasks = _selectedProject.Tasks.Where(x => !x.isDone || ShowAllTasks).ToList();

          
        }
        #endregion
    }
}
