using MediFlow.App.Helpers;

namespace MediFlow.App.Helpers;

public static class UIHelper
{
    public static void StyleGrid(DataGridView grid)
    {
        grid.BackgroundColor = Theme.Surface;
        grid.BorderStyle = BorderStyle.None;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.GridColor = Theme.Border;
        grid.RowHeadersVisible = false;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.ReadOnly = true;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        grid.ColumnHeadersHeight = 40;
        grid.RowTemplate.Height = 38;
        grid.Font = Theme.FontRegular;
        grid.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#DBEAFE");
        grid.DefaultCellStyle.SelectionForeColor = Theme.TextPrimary;
        grid.DefaultCellStyle.BackColor = Theme.Surface;
        grid.DefaultCellStyle.ForeColor = Theme.TextPrimary;
        grid.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
        grid.AlternatingRowsDefaultCellStyle.BackColor = Theme.GridAltRow;
        grid.ColumnHeadersDefaultCellStyle.BackColor = Theme.GridHeader;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.Font = Theme.FontMedium;
        grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
    }

    public static Button MakeButton(string text, Color backColor, int width = 110, int height = 36)
    {
        return new Button
        {
            Text = text,
            BackColor = backColor,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = Theme.FontMedium,
            Size = new Size(width, height),
            Cursor = Cursors.Hand,
            FlatAppearance = { BorderSize = 0 }
        };
    }

    public static Panel MakeStatCard(string title, string value, Color accent)
    {
        var card = new Panel
        {
            BackColor = Theme.Surface,
            Size = new Size(190, 90),
            Padding = new Padding(14)
        };

        var bar = new Panel
        {
            BackColor = accent,
            Dock = DockStyle.Left,
            Width = 5
        };

        var lblValue = new Label
        {
            Text = value,
            Font = new Font("Segoe UI", 22f, FontStyle.Bold),
            ForeColor = accent,
            AutoSize = false,
            Size = new Size(140, 36),
            Location = new Point(18, 12),
            TextAlign = ContentAlignment.MiddleLeft
        };

        var lblTitle = new Label
        {
            Text = title,
            Font = Theme.FontSmall,
            ForeColor = Theme.TextSecondary,
            AutoSize = false,
            Size = new Size(150, 20),
            Location = new Point(18, 52),
            TextAlign = ContentAlignment.MiddleLeft
        };

        card.Controls.AddRange(new Control[] { bar, lblValue, lblTitle });
        return card;
    }

    public static TextBox MakeSearchBox(string placeholder)
    {
        var tb = new TextBox
        {
            Font = Theme.FontRegular,
            ForeColor = Theme.TextSecondary,
            BackColor = Theme.Surface,
            BorderStyle = BorderStyle.FixedSingle,
            Text = placeholder,
            Height = 32
        };
        tb.GotFocus += (s, e) => { if (tb.Text == placeholder) { tb.Text = ""; tb.ForeColor = Theme.TextPrimary; } };
        tb.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(tb.Text)) { tb.Text = placeholder; tb.ForeColor = Theme.TextSecondary; } };
        return tb;
    }

    public static void ShowSuccess(string message) =>
        MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

    public static void ShowError(string message) =>
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

    public static bool ConfirmDelete(string item) =>
        MessageBox.Show($"Are you sure you want to delete {item}?\nThis cannot be undone.",
            "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
}
