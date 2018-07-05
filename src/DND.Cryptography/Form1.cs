using DND.Common.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DND.Cryptography
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGenerateRSAKeys_Click(object sender, EventArgs e)
        {
            var result = AsymmetricEncryptionHelper.RsaWithPEMKeyString.AssignNewKey();
            txtPrivateKey.Text = result.privateKey;
            txtPublicKey.Text = result.publicKey;
        }

        private void btnSaveRSAPublicKey_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            // set a default file name
            savefile.FileName = "public.x509.pem";
            // set filters - this can be done in properties as well
            savefile.Filter = "Public Key files (*.pub)|*.pub|All files (*.*)|*.*";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(savefile.FileName, txtPublicKey.Text);
            }
        }

        private void btnSaveRSAPrivateKey_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            // set a default file name
            savefile.FileName = "private.rsa.pem";
            // set filters - this can be done in properties as well
            savefile.Filter = "Private Key files (*.key)|*.key|All files (*.*)|*.*";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(savefile.FileName, txtPrivateKey.Text);
            }
        }
    }
}
