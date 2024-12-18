namespace BillValidatorEmulator
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cmbPorts = new System.Windows.Forms.ComboBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnEnable = new System.Windows.Forms.Button();
            this.btnDisable = new System.Windows.Forms.Button();
            this.btnStack = new System.Windows.Forms.Button();
            this.btnReturn = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pnlProcessingLed = new System.Windows.Forms.Panel();
            this.pnlReadyLed = new System.Windows.Forms.Panel();
            this.pnlErrorLed = new System.Windows.Forms.Panel();
            this.pnlPowerLed = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pnlReturnSensor = new System.Windows.Forms.Panel();
            this.pnlStackingSensor = new System.Windows.Forms.Panel();
            this.pnlValidatingSensor = new System.Windows.Forms.Panel();
            this.pnlEntrySensor = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbPorts
            // 
            this.cmbPorts.FormattingEnabled = true;
            this.cmbPorts.Location = new System.Drawing.Point(12, 12);
            this.cmbPorts.Name = "cmbPorts";
            this.cmbPorts.Size = new System.Drawing.Size(121, 23);
            this.cmbPorts.TabIndex = 0;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(139, 12);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "Conectar";
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnReset
            // 
            this.btnReset.Enabled = false;
            this.btnReset.Location = new System.Drawing.Point(12, 41);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 2;
            this.btnReset.Text = "Reset";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnEnable
            // 
            this.btnEnable.Enabled = false;
            this.btnEnable.Location = new System.Drawing.Point(93, 41);
            this.btnEnable.Name = "btnEnable";
            this.btnEnable.Size = new System.Drawing.Size(75, 23);
            this.btnEnable.TabIndex = 3;
            this.btnEnable.Text = "Habilitar";
            this.btnEnable.Click += new System.EventHandler(this.btnEnable_Click);
            // 
            // btnDisable
            // 
            this.btnDisable.Enabled = false;
            this.btnDisable.Location = new System.Drawing.Point(174, 41);
            this.btnDisable.Name = "btnDisable";
            this.btnDisable.Size = new System.Drawing.Size(75, 23);
            this.btnDisable.TabIndex = 4;
            this.btnDisable.Text = "Deshabilitar";
            this.btnDisable.Click += new System.EventHandler(this.btnDisable_Click);
            // 
            // btnStack
            // 
            this.btnStack.Enabled = false;
            this.btnStack.Location = new System.Drawing.Point(255, 41);
            this.btnStack.Name = "btnStack";
            this.btnStack.Size = new System.Drawing.Size(75, 23);
            this.btnStack.TabIndex = 5;
            this.btnStack.Text = "Stack";
            this.btnStack.Click += new System.EventHandler(this.btnStack_Click);
            // 
            // btnReturn
            // 
            this.btnReturn.Enabled = false;
            this.btnReturn.Location = new System.Drawing.Point(336, 41);
            this.btnReturn.Name = "btnReturn";
            this.btnReturn.Size = new System.Drawing.Size(75, 23);
            this.btnReturn.TabIndex = 6;
            this.btnReturn.Text = "Return";
            this.btnReturn.Click += new System.EventHandler(this.btnReturn_Click);
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(12, 266);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(399, 183);
            this.txtLog.TabIndex = 7;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pnlProcessingLed);
            this.groupBox1.Controls.Add(this.pnlReadyLed);
            this.groupBox1.Controls.Add(this.pnlErrorLed);
            this.groupBox1.Controls.Add(this.pnlPowerLed);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 70);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(399, 90);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.Text = "LEDs de Estado";
            // 
            // pnlProcessingLed
            // 
            this.pnlProcessingLed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlProcessingLed.Location = new System.Drawing.Point(324, 45);
            this.pnlProcessingLed.Name = "pnlProcessingLed";
            this.pnlProcessingLed.Size = new System.Drawing.Size(30, 30);
            this.pnlProcessingLed.TabIndex = 7;
            // 
            // pnlReadyLed
            // 
            this.pnlReadyLed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlReadyLed.Location = new System.Drawing.Point(219, 45);
            this.pnlReadyLed.Name = "pnlReadyLed";
            this.pnlReadyLed.Size = new System.Drawing.Size(30, 30);
            this.pnlReadyLed.TabIndex = 6;
            // 
            // pnlErrorLed
            // 
            this.pnlErrorLed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlErrorLed.Location = new System.Drawing.Point(114, 45);
            this.pnlErrorLed.Name = "pnlErrorLed";
            this.pnlErrorLed.Size = new System.Drawing.Size(30, 30);
            this.pnlErrorLed.TabIndex = 5;
            // 
            // pnlPowerLed
            // 
            this.pnlPowerLed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlPowerLed.Location = new System.Drawing.Point(9, 45);
            this.pnlPowerLed.Name = "pnlPowerLed";
            this.pnlPowerLed.Size = new System.Drawing.Size(30, 30);
            this.pnlPowerLed.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(324, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 15);
            this.label4.TabIndex = 3;
            this.label4.Text = "Procesando";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(219, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Listo";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(114, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Error";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Encendido";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pnlReturnSensor);
            this.groupBox2.Controls.Add(this.pnlStackingSensor);
            this.groupBox2.Controls.Add(this.pnlValidatingSensor);
            this.groupBox2.Controls.Add(this.pnlEntrySensor);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(12, 166);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(399, 90);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.Text = "Estado de Sensores";
            // 
            // pnlReturnSensor
            // 
            this.pnlReturnSensor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlReturnSensor.Location = new System.Drawing.Point(324, 45);
            this.pnlReturnSensor.Name = "pnlReturnSensor";
            this.pnlReturnSensor.Size = new System.Drawing.Size(30, 30);
            this.pnlReturnSensor.TabIndex = 7;
            // 
            // pnlStackingSensor
            // 
            this.pnlStackingSensor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlStackingSensor.Location = new System.Drawing.Point(219, 45);
            this.pnlStackingSensor.Name = "pnlStackingSensor";
            this.pnlStackingSensor.Size = new System.Drawing.Size(30, 30);
            this.pnlStackingSensor.TabIndex = 6;
            // 
            // pnlValidatingSensor
            // 
            this.pnlValidatingSensor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlValidatingSensor.Location = new System.Drawing.Point(114, 45);
            this.pnlValidatingSensor.Name = "pnlValidatingSensor";
            this.pnlValidatingSensor.Size = new System.Drawing.Size(30, 30);
            this.pnlValidatingSensor.TabIndex = 5;
            // 
            // pnlEntrySensor
            // 
            this.pnlEntrySensor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlEntrySensor.Location = new System.Drawing.Point(9, 45);
            this.pnlEntrySensor.Name = "pnlEntrySensor";
            this.pnlEntrySensor.Size = new System.Drawing.Size(30, 30);
            this.pnlEntrySensor.TabIndex = 4;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(324, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(44, 15);
            this.label8.TabIndex = 3;
            this.label8.Text = "Return";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(219, 25);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(51, 15);
            this.label7.TabIndex = 2;
            this.label7.Text = "Stacking";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(114, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 15);
            this.label6.TabIndex = 1;
            this.label6.Text = "Validating";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 15);
            this.label5.TabIndex = 0;
            this.label5.Text = "Entry";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 461);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnReturn);
            this.Controls.Add(this.btnStack);
            this.Controls.Add(this.btnDisable);
            this.Controls.Add(this.btnEnable);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.cmbPorts);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bill Validator Emulator";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.ComboBox cmbPorts;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnEnable;
        private System.Windows.Forms.Button btnDisable;
        private System.Windows.Forms.Button btnStack;
        private System.Windows.Forms.Button btnReturn;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel pnlProcessingLed;
        private System.Windows.Forms.Panel pnlReadyLed;
        private System.Windows.Forms.Panel pnlErrorLed;
        private System.Windows.Forms.Panel pnlPowerLed;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel pnlReturnSensor;
        private System.Windows.Forms.Panel pnlStackingSensor;
        private System.Windows.Forms.Panel pnlValidatingSensor;
        private System.Windows.Forms.Panel pnlEntrySensor;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
    }
}
