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
    public partial class FrmBulkUpload : Form
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

        private class AlbumEntry
        {
            public string DisplayName { get; private set; }
            public string Id { get; private set; }
            public AlbumEntry(ImgurAlbum A)
            {
                DisplayName = A.title;
                if (string.IsNullOrEmpty(DisplayName))
                {
                    DisplayName = "ID=" + A.id;
                }
                Id = A.id;
            }

            public override string ToString()
            {
                return DisplayName;
            }
        }

        private Settings S;

        public FrmBulkUpload(Settings S)
        {
            this.S = S;
            InitializeComponent();
        }

        private void FillAlbums()
        {
            var I = new Imgur(S);
            var Albums = new List<ImgurAlbum>();
            Invoke((MethodInvoker)delegate
            {
                cbAlbum.Enabled = false;
                cbAlbum.Items.Clear();
                cbAlbum.Items.Add("Working...");
                cbAlbum.Items.Add("<none>");
                cbAlbum.SelectedIndex = 0;
            });
            Albums.AddRange(I.GetAccountAlbums().OrderBy(m => m.title));
            Invoke((MethodInvoker)delegate
            {
                cbAlbum.Items.RemoveAt(0);
                cbAlbum.SelectedIndex = 0;
                foreach (var A in Albums)
                {
                    cbAlbum.Items.Add(new AlbumEntry(A));
                }
                cbAlbum.Enabled = true;
            });
        }

        private void BtnAddImages_Click(object sender, EventArgs e)
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

        private void LbFileList_KeyDown(object sender, KeyEventArgs e)
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

        private async void BtnStartUpload_Click(object sender, EventArgs e)
        {
            cbAlbum.Enabled = btnStartUpload.Enabled = btnAddImages.Enabled = lbFileList.Enabled = false;
            var ItemList = new Stack<FileNameItem>(lbFileList.Items.OfType<FileNameItem>().Reverse());
            var I = new Imgur(S);

            if (ItemList.Count > 0)
            {
                string AlbumId = cbAlbum.SelectedIndex > 0 ? ((AlbumEntry)cbAlbum.SelectedItem).Id : null;
                List<string> Images = AlbumId == null ? null : (await I.GetAlbumImages(AlbumId)).Select(m => m.id).ToList();

                while (ItemList.Count > 0)
                {
                    var Current = ItemList.Pop();
                    var Img = await I.UploadImage(
                        File.ReadAllBytes(Current.LongName),
                        Path.GetFileName(Current.LongName),
                        FormatFileString(tbTitle.Text, Current.LongName),
                        cbDescDate.Checked ? DateTime.UtcNow.ToString(@"yyyy-MM-dd HH:mm:ss \U\T\C") : "");
                    if (Img != null)
                    {
                        if (Images != null)
                        {
                            Images.Add(Img.id);
                        }
                        Cache.GetImage(Img);
                        lbFileList.Items.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
                if (Images != null)
                {
                    await I.SetAlbumImages(AlbumId, Images);
                }
                cbAlbum.Enabled = btnStartUpload.Enabled = btnAddImages.Enabled = lbFileList.Enabled = true;
                if (lbFileList.Items.Count == 0)
                {
                    DialogResult = DialogResult.OK;
                }
            }
            else
            {
                MessageBox.Show("Please select at least one file to upload.", "No files", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void FrmBulkUpload_Load(object sender, EventArgs e)
        {
            Thread T = new Thread(FillAlbums)
            {
                IsBackground = true
            };
            T.Start();
        }

        private static string FormatFileString(string Format, string FullFileName)
        {
            var FileName = Path.GetFileName(FullFileName);
            var NameOnly = Path.GetFileNameWithoutExtension(FileName);
            var Ext = NameOnly.Length == FileName.Length ? string.Empty : FileName.Substring(NameOnly.Length + 1);
            return Format
                .Replace("%N", NameOnly)
                .Replace("%X", Ext);
        }
    }
}
