using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
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

        private readonly Settings S;

        public FrmBulkUpload(Settings S)
        {
            this.S = S;
            InitializeComponent();
        }

        private void FillAlbums()
        {
            Imgur I = new Imgur(S);
            List<ImgurAlbum> Albums = new List<ImgurAlbum>();
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
                foreach (ImgurAlbum A in Albums)
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
                FileNameItem[] Names = OFD.FileNames.Select(m => new FileNameItem(m)).ToArray();
                FileNameItem[] Existing = lbFileList.Items.OfType<FileNameItem>().ToArray();
                foreach (FileNameItem Item in Names)
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
            int Index = lbFileList.SelectedIndex;
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
                        object Item = lbFileList.SelectedItem;
                        lbFileList.Items.Remove(Item);
                        lbFileList.Items.Insert(--Index, Item);
                    }
                }
                if (e.KeyCode == Keys.Down && e.Modifiers == Keys.Alt)
                {
                    if (Index < lbFileList.Items.Count - 1)
                    {
                        object Item = lbFileList.SelectedItem;
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
            SetControlState(false);
            Stack<FileNameItem> ItemList = new Stack<FileNameItem>(lbFileList.Items.OfType<FileNameItem>().Reverse());
            Imgur I = new Imgur(S);

            if (ItemList.Count > 0)
            {
                string AlbumId = cbAlbum.SelectedIndex > 0 ? ((AlbumEntry)cbAlbum.SelectedItem).Id : null;
                List<string> Images = AlbumId == null ? null : (await I.GetAlbumImages(AlbumId)).Select(m => m.id).ToList();

                lbFileList.SelectedIndex = 0;
                while (ItemList.Count > 0)
                {
                    FileNameItem Current = ItemList.Pop();
                    ImgurImage Img = await I.UploadImage(
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
                        //Add failed image back to the queue
                        ItemList.Push(Current);
                        //Ask user to retry or cancel
                        if (MessageBox.Show("Failed to upload an image. Response: " + I.LastError, "Upload failed", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
                        {
                            break;
                        }
                    }
                }
                if (Images != null)
                {
                    while (
                        !await I.SetAlbumImages(AlbumId, Images) &&
                        MessageBox.Show("Failed to add uploaded images to the album. Response: " + I.LastError, "Album change failed", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation) == DialogResult.Retry)
                    {
                        ;
                    }
                }
                if (lbFileList.Items.Count == 0)
                {
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    SetControlState(true);
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

        private void SetControlState(bool State)
        {
            tbTitle.Enabled =
                cbDescDate.Enabled =
                cbAlbum.Enabled =
                btnStartUpload.Enabled =
                btnAddImages.Enabled =
                lbFileList.Enabled =
                State;
        }

        private static string FormatFileString(string Format, string FullFileName)
        {
            string FileName = Path.GetFileName(FullFileName);
            string NameOnly = Path.GetFileNameWithoutExtension(FileName);
            string Ext = NameOnly.Length == FileName.Length ? string.Empty : FileName.Substring(NameOnly.Length + 1);
            return Format
                .Replace("%N", NameOnly)
                .Replace("%X", Ext);
        }
    }
}
