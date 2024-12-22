// © 2024 led-mirage. All rights reserved.

using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace PingMonitor;

public partial class MainForm : Form
{
    public const string AppName = "PingMonitor";
    public const string AppVersion = "1.0.0";
    public const string Copyright = "© 2024 led-mirage";

    private int currentHostGroupIndex = 0;
    private List<HostGroup> hostGroups = new List<HostGroup>();
    private Label[] labels = Array.Empty<Label>();
    private PictureBox[] pictureBoxes = Array.Empty<PictureBox>();
    private FlowLayoutPanel? flowLayoutPanel;
    private Control? statusTable;
    private Image? okImage;
    private Image? ngImage;
    private CancellationTokenSource? pingCancelTokenSource;
    private Task? pingTask = null;

    private const int WM_SYSCOMMAND = 0x112;
    private const int MF_STRING = 0x0;
    private const uint MF_SEPARATOR = 0x800;
    private const int ABOUT_MENU_ID = 0x1;

    [DllImport("user32.dll")]
    private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

    [DllImport("user32.dll")]
    private static extern bool InsertMenu(IntPtr hMenu, uint uPosition, uint uFlags, uint uIDNewItem, string lpNewItem);

    public MainForm()
    {
        hostGroups = HostFileHelper.ParseHostFile("hosts.txt");

        int currentGroupIndex = Properties.Settings.Default.CurrentGroupIndex;
        if (currentGroupIndex >= hostGroups.Count) {
            currentGroupIndex = 0;
        }
        this.currentHostGroupIndex = currentGroupIndex;

        LoadImage();
        InitializeComponent();
        this.Icon = new Icon("images/icon/PingMonitor.ico");
        this.AutoScaleMode = AutoScaleMode.Dpi;
        this.Text = $"{AppName} {AppVersion}";
        SetupUI();

        StartPingMonitoring(hostGroups[currentHostGroupIndex]);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        int width = Properties.Settings.Default.FormWidth;
        int height = Properties.Settings.Default.FormHeight;
        int left = Properties.Settings.Default.FormLeft;
        int top = Properties.Settings.Default.FormTop;

        if (width > 0 && height > 0)
        {
            this.Width = width;
            this.Height = height;
        }
        if (left >= 0 && top >= 0)
        {
            this.Left = left;
            this.Top = top;
        }

        IntPtr systemMenu = GetSystemMenu(this.Handle, false);
        const uint SC_CLOSE = 0xF060;
        string menuText = string.Format(LocalizationManager.GetString("AboutMenu"), AppName);
        InsertMenu(systemMenu, SC_CLOSE, MF_STRING, ABOUT_MENU_ID, menuText);
        InsertMenu(systemMenu, SC_CLOSE, MF_SEPARATOR, 0, "");
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        Properties.Settings.Default.FormWidth = this.Width;
        Properties.Settings.Default.FormHeight = this.Height;
        Properties.Settings.Default.FormLeft = this.Left;
        Properties.Settings.Default.FormTop = this.Top;
        Properties.Settings.Default.CurrentGroupIndex = this.currentHostGroupIndex;
        Properties.Settings.Default.Save();

        base.OnFormClosing(e);
    }

    protected override void OnDeactivate(EventArgs e)
    {
        base.OnDeactivate(e);

        // Remove focus when the form becomes inactive
        this.ActiveControl = null;
    }

    private void LoadImage()
    {
        try
        {
            this.okImage = ImageHelper.ResizeImageWithAntialiasing(Image.FromFile("Images/ok.png"), this.ScaleSize(new Size(16, 16)));
            this.ngImage = ImageHelper.ResizeImageWithAntialiasing(Image.FromFile("Images/ng.png"), this.ScaleSize(new Size(16, 16)));
        }
        catch(FileNotFoundException e)
        {
            string errorTitle = LocalizationManager.GetString("ErrorTitle");
            string errorMessage = $"{LocalizationManager.GetString("ErrorImageNotFound")}\n{e.Message}";
            
            MessageBox.Show(errorMessage, errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.ExitThread();
            Environment.Exit(1);
        }
    }

    private void SetupUI()
    {
        flowLayoutPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };

        if (hostGroups.Count > 1)
        {
            SetupGroupSelectControl();
        }
        SetupStatusTable(hostGroups[currentHostGroupIndex]);

        Controls.Add(flowLayoutPanel);
    }

    private void SetupGroupSelectControl()
    {
        var groupSelectControl = new GroupSelectControl
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            CurrentIndex = this.currentHostGroupIndex,
            GroupNames = hostGroups.Select(x => x.GroupName).ToList()
        };
        groupSelectControl.GroupChanged += GroupSelectControl_GroupChanged;
        flowLayoutPanel?.Controls.Add(groupSelectControl);
    }

    private void SetupStatusTable(HostGroup hostGroup)
    {
        int baseFontSize = AppConfig.FontSize;

        var tableLayoutPanel = new TableLayoutPanel
        {
            ColumnCount = 2,
            RowCount = hostGroup.HostNames.Count,
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };

        for (int i = 0; i < hostGroup.HostNames.Count; i++)
        {
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        }

        labels = new Label[hostGroup.HostNames.Count];

        pictureBoxes = new PictureBox[hostGroup.HostNames.Count];
        for (int i = 0; i < hostGroup.HostNames.Count; i++)
        {
            pictureBoxes[i] = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
                Width = this.ScaleValue((int)(baseFontSize * 1.5)),
                Height = this.ScaleValue((int)(baseFontSize * 1.5)),
                Margin = new Padding(this.ScaleValue(baseFontSize / 2)),
                Anchor = AnchorStyles.None
            };
            tableLayoutPanel.Controls.Add(pictureBoxes[i], 0, i);

            labels[i] = new Label
            {
                AutoSize = true,
                Text = $"{hostGroup.HostNames[i]}",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(this.ScaleValue(baseFontSize / 2) ,0, 0, this.ScaleValue(baseFontSize / 4)),
                Anchor = AnchorStyles.Left | AnchorStyles.Top,
                Font = new Font(AppConfig.FontFamily, baseFontSize)
            };
            tableLayoutPanel.Controls.Add(labels[i], 1, i);
        }

        statusTable = tableLayoutPanel;
        flowLayoutPanel?.Controls.Add(statusTable);
    }

    private void DisposeStatusTable()
    {
        if (statusTable != null)
        {
            statusTable.Dispose();
            statusTable = null;
        }

        labels = Array.Empty<Label>();
        pictureBoxes = Array.Empty<PictureBox>();
    }

    private void GroupSelectControl_GroupChanged(object? sender, int groupIndex)
    {
        StopPingMonitoring();
        DisposeStatusTable();

        currentHostGroupIndex = groupIndex;

        SetupStatusTable(hostGroups[currentHostGroupIndex]);
        StartPingMonitoring(hostGroups[currentHostGroupIndex]);
    }

    private void StartPingMonitoring(HostGroup hostGroup)
    {
        pingTask = Task.Run(() => PingMonitoringTask(hostGroup));
    }

    private async Task PingMonitoringTask(HostGroup hostGroup)
    {
        pingCancelTokenSource = new CancellationTokenSource();

        int intervalMs = AppConfig.IntervalMs;
        int retryCount = AppConfig.RetryCount;
        int timeout = AppConfig.PingTimeoutMs;

        try
        {
            while (true)
            {
                pingCancelTokenSource.Token.ThrowIfCancellationRequested();

                var hosts = hostGroup.HostNames;
                var pingTasks = new Task[hosts.Count];

                for (int i = 0; i < hosts.Count; i++)
                {
                    int index = i;
                    pingTasks[i] = Task.Run(async () =>
                    {
                        bool isAlive = false;
                        try
                        {
                            PingReply pingReply = await PingHelper.SendPingWithRetryAsync(hosts[index], timeout, retryCount, pingCancelTokenSource.Token);
                            isAlive = pingReply.Status == IPStatus.Success;
                        }
                        catch (Exception)
                        {
                            isAlive = false;
                        }
                        if (!pingCancelTokenSource.Token.IsCancellationRequested)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                if (labels.Length <= index || pictureBoxes.Length <= index) return;
                                labels[index].Text = $"{hosts[index]}";
                                pictureBoxes[index].Image = isAlive ? okImage : ngImage;
                            });
                        }
                    });
                }

                await Task.WhenAll(pingTasks);

                await Task.Delay(intervalMs, pingCancelTokenSource.Token);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void StopPingMonitoring()
    {
        pingCancelTokenSource?.Cancel();
        pingTask?.Wait();
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_SYSCOMMAND)
        {
            if ((int)m.WParam == ABOUT_MENU_ID)
            {
                AboutDialog aboutDialog = new AboutDialog();
                aboutDialog.ShowInTaskbar = false;
                aboutDialog.ShowDialog(this);
                return;
            }
        }

        base.WndProc(ref m);
    }
}
