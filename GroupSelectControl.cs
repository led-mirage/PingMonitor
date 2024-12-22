// © 2024 led-mirage. All rights reserved.

public class GroupSelectControl : UserControl
{
    private FlowLayoutPanel? flowLayoutPanel;
    private Button? prevButton;
    private Button? nextButton;
    private Label? groupNameLabel;
    private Image? prevImage;
    private Image? nextImage;

    private List<string> groupNames = new List<string>();
    private int currentIndex = 0;

    public GroupSelectControl()
    {
        LoadImage();
        InitializeComponent();
    }

    public List<string> GroupNames
    {
        get { return groupNames; }
        set
        {
            groupNames = value;
            UpdateGroupName();
        }
    }

    public int CurrentIndex
    {
        get { return this.currentIndex; }
        set { this.currentIndex = value; }
    }

    public event EventHandler<int>? GroupChanged;

    private void LoadImage()
    {
        try
        {
            this.prevImage = ImageHelper.ResizeImageWithAntialiasing(Image.FromFile("images/prev.png"), this.ScaleSize(new Size(10, 10)));
            this.nextImage = ImageHelper.ResizeImageWithAntialiasing(Image.FromFile("images/next.png"), this.ScaleSize(new Size(10, 10)));
        }
        catch (FileNotFoundException e)
        {
            string errorTitle = LocalizationManager.GetString("ErrorTitle");
            string errorMessage = $"{LocalizationManager.GetString("ErrorImageNotFound")}\n{e.Message}";
            
            MessageBox.Show(errorMessage, errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.ExitThread();
            Environment.Exit(1);
        }
    }

    private void InitializeComponent()
    {
        int baseFontSize = AppConfig.FontSize;

        flowLayoutPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(0),
        };

        prevButton = new Button
        {
            Text = "",
            Image = prevImage,
            ImageAlign = ContentAlignment.MiddleCenter,
            Size = this.ScaleSize(new Size((int)(baseFontSize * 1.5), (int)(baseFontSize * 1.5))),
            FlatStyle = FlatStyle.Flat,
            Margin = new Padding(baseFontSize, 0, 0, 0),
            Anchor = AnchorStyles.None
        };
        prevButton.FlatAppearance.BorderSize = 0;

        nextButton = new Button
        {
            Text = "",
            Image = nextImage,
            ImageAlign = ContentAlignment.MiddleCenter,
            Size = this.ScaleSize(new Size((int)(baseFontSize * 1.5), (int)(baseFontSize * 1.5))),
            FlatStyle = FlatStyle.Flat,
            Margin = new Padding(0),
            Anchor = AnchorStyles.None
        };
        nextButton.FlatAppearance.BorderSize = 0;

        groupNameLabel = new Label
        {
            Text = "",
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(baseFontSize / 2),
            Anchor = AnchorStyles.Left | AnchorStyles.Top,
            Font = new Font(AppConfig.FontFamily, baseFontSize)
        };

        flowLayoutPanel.Controls.Add(prevButton);
        flowLayoutPanel.Controls.Add(nextButton);
        flowLayoutPanel.Controls.Add(groupNameLabel);
        this.Controls.Add(flowLayoutPanel);

        prevButton.Click += PrevButton_Click;
        nextButton.Click += NextButton_Click;
    }

    private void PrevButton_Click(object? sender, EventArgs e)
    {
        if (groupNames.Count == 0) return;

        currentIndex = (currentIndex == 0) ? groupNames.Count - 1 : currentIndex - 1;
        UpdateGroupName();
    }

    private void NextButton_Click(object? sender, EventArgs e)
    {
        if (groupNames.Count == 0) return;

        currentIndex = (currentIndex + 1) % groupNames.Count;
        UpdateGroupName();
    }

    private void UpdateGroupName()
    {
        if (groupNameLabel == null) return;

        if (groupNames.Count > 0)
        {
            groupNameLabel.Text = $"{groupNames[currentIndex]}";
            GroupChanged?.Invoke(this, currentIndex);
        }
        else
        {
            groupNameLabel.Text = "";
        }
    }
}
