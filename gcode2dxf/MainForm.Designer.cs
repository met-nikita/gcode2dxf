
namespace gcode2dxf
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.bt_convert = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bt_convert
            // 
            this.bt_convert.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_convert.Location = new System.Drawing.Point(12, 12);
            this.bt_convert.Name = "bt_convert";
            this.bt_convert.Size = new System.Drawing.Size(273, 82);
            this.bt_convert.TabIndex = 0;
            this.bt_convert.Text = "Конвертировать файл";
            this.bt_convert.UseVisualStyleBackColor = true;
            this.bt_convert.Click += new System.EventHandler(this.ConvertClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(297, 106);
            this.Controls.Add(this.bt_convert);
            this.Name = "MainForm";
            this.Text = "gcode2dxf";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bt_convert;
    }
}

