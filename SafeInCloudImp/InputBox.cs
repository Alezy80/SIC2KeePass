using System;
using System.Drawing;
using System.Windows.Forms;

namespace SafeInCloudImp
{
    /// <summary>
    /// Форма запроса значения
    /// </summary>
    public sealed class InputBox : Form
    {
        private readonly Button _buttonCancel;
        private readonly Button _buttonOK;
        private readonly Label _label;
        private readonly TextBox _textValue;

        private InputBox(string caption, string text)
        {
            _label = new Label();
            _textValue = new TextBox();
            _buttonOK = new Button();
            _buttonCancel = new Button();
            SuspendLayout();
            _label.AutoSize = true;
            _label.Location = new Point(9, 13);
            _label.Name = "label";
            _label.Size = new Size(31, 13);
            _label.TabIndex = 1;
            _label.Text = text;
            _textValue.Location = new Point(12, 31);
            _textValue.Name = "textValue";
            _textValue.Size = new Size(245, 20);
            _textValue.TabIndex = 2;
            _textValue.WordWrap = false;
            _textValue.PasswordChar = '*';
            _buttonOK.DialogResult = DialogResult.OK;
            _buttonOK.Location = new Point(57, 67);
            _buttonOK.Name = "buttonOK";
            _buttonOK.Size = new Size(75, 23);
            _buttonOK.TabIndex = 3;
            _buttonOK.Text = "OK";
            _buttonOK.UseVisualStyleBackColor = true;
            _buttonCancel.DialogResult = DialogResult.Cancel;
            _buttonCancel.Location = new Point(138, 67);
            _buttonCancel.Name = "buttonCancel";
            _buttonCancel.Size = new Size(75, 23);
            _buttonCancel.TabIndex = 4;
            _buttonCancel.Text = "Отмена";
            _buttonCancel.UseVisualStyleBackColor = true;
            AcceptButton = _buttonOK;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = _buttonCancel;
            ClientSize = new Size(270, 103);
            Controls.Add(_buttonCancel);
            Controls.Add(_buttonOK);
            Controls.Add(_textValue);
            Controls.Add(_label);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "InputBox";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = caption;
            ResumeLayout(false);
            PerformLayout();
        }

        /// <summary>
        /// Запрос значения
        /// </summary>
        /// <param name="caption">Заголовок</param>
        /// <param name="text">Поясняющий текст</param>
        /// <param name="val">Первоначальное/возвращаемое значение</param>
        /// <returns>Значение было введено?</returns>
        public static bool Query(string caption, string text, ref string val)
        {
            using (var ib = new InputBox(caption, text))
            {
                ib._textValue.Text = val;
                if (ib.ShowDialog() != DialogResult.OK)
                    return false;
                val = ib._textValue.Text;
                return true;
            }
        }
    }
}