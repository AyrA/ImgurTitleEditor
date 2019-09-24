using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImgurTitleEditor
{
    public partial class frmBulkUpload : Form
    {
        private struct FileNameItem
        {
            public string ShortName
            {
                get
                {
                    return LongName.Length > 100 ? "..." + LongName.Substring(LongName.Length - 100, 100) : LongName;
                }
            }

            public string LongName
            {
                get; private set;
            }

            public FileNameItem(string FullName)
            {
                LongName = FullName;
            }

            public override int GetHashCode()
            {
                return LongName.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return obj != null &&
                    obj is FileNameItem &&
                    obj.GetHashCode() == GetHashCode();
            }

            public override string ToString()
            {
                return ShortName;
            }
        }

        private Settings S;

        public frmBulkUpload(Settings S)
        {
            this.S = S;
            InitializeComponent();
        }

        private void btnAddImages_Click(object sender, EventArgs e)
        {
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                var Names = OFD.FileNames.Select(m => new FileNameItem(m)).ToArray();
                var Existing = lbFileList.Items.OfType<FileNameItem>().ToArray();
                foreach (var Item in Names)
                {
                    if (Existing.All(m => !m.Equals(Item)))
                    {
                        lbFileList.Items.Add(Item);
                    }
                }
            }
        }

        private void lbFileList_KeyDown(object sender, KeyEventArgs e)
        {
            var Index = lbFileList.SelectedIndex;
            if (Index >= 0)
            {
                if (e.KeyCode == Keys.Delete)
                {
                    lbFileList.Items.RemoveAt(Index);
                }
                if (e.KeyCode == Keys.Up && e.Modifiers == Keys.Alt)
                {
                    if (Index > 0)
                    {
                        var Item = lbFileList.SelectedItem;
                        lbFileList.Items.Remove(Item);
                        lbFileList.Items.Insert(--Index, Item);
                    }
                }
                if (e.KeyCode == Keys.Down && e.Modifiers == Keys.Alt)
                {
                    if (Index < lbFileList.Items.Count - 1)
                    {
                        var Item = lbFileList.SelectedItem;
                        lbFileList.Items.Remove(Item);
                        lbFileList.Items.Insert(++Index, Item);
                    }
                }
                if (lbFileList.Items.Count > 0)
                {
                    lbFileList.SelectedIndex = Math.Min(Index, lbFileList.Items.Count - 1);
                }
            }
        }

        private void btnStartUpload_Click(object sender, EventArgs e)
        {
            btnStartUpload.Enabled = btnAddImages.Enabled = lbFileList.Enabled = false;
            var ItemList = new Stack<FileNameItem>(lbFileList.Items.OfType<FileNameItem>().Reverse());
            var I = new Imgur(S);

            if (ItemList.Count > 0)
            {
                Thread T = new Thread(delegate ()
                {
                    while (ItemList.Count > 0)
                    {
                        var Current = ItemList.Pop();
                        Cache.GetImage(I.UploadImage(
                            File.ReadAllBytes(Current.LongName),
                            Path.GetFileName(Current.LongName),
                            Path.GetFileNameWithoutExtension(Current.LongName),
                            DateTime.UtcNow.ToString(@"yyyy-MM-dd HH:mm:ss \U\T\C")
                            ).Result);
                        Invoke((MethodInvoker)delegate
                        {
                            lbFileList.Items.RemoveAt(0);
                        });
                    }
                    Invoke((MethodInvoker)delegate
                    {
                        btnStartUpload.Enabled = btnAddImages.Enabled = lbFileList.Enabled = true;
                        DialogResult = DialogResult.OK;
                    });
                });
                T.IsBackground = true;
                T.Start();
            }
            else
            {
                MessageBox.Show("Please select at least one file to upload.", "No files", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
