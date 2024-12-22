// © 2024 led-mirage. All rights reserved.

namespace PingMonitor;

public class AboutDialog : Form
{
    public AboutDialog()
    {
        // ウィンドウの設定
        this.Text = MainForm.AppName;
        this.Size = this.ScaleSize(new Size(200, 100));
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.StartPosition = FormStartPosition.CenterParent;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        // コンテンツ
        Label aboutLabel = new Label
        {
            Text = $"{MainForm.AppName} {MainForm.AppVersion}\n{MainForm.Copyright}",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font(AppConfig.FontFamily, 9)
        };

        // コントロールを追加
        this.Controls.Add(aboutLabel);

        // ESCキーでダイアログを閉じる
        this.KeyPreview = true;
        this.KeyDown += (sender, e) =>
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        };
    }
}
