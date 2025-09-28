using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace File_Organizer
{
    public partial class MainWindow : Window
    {
        private string selectedFolderPath = "";
        private string selectedFilePath = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.ValidateNames = false;
            dialog.CheckFileExists = false;
            dialog.CheckPathExists = true;
            dialog.FileName = "Выберите папку";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                selectedFolderPath = Path.GetDirectoryName(dialog.FileName);
                FolderPathLabel.Content = selectedFolderPath;
                UpdatePreview();
            }
        }

        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Выберите файл";
            dialog.Filter = "Все файлы (*.*)|*.*|Текстовые файлы (*.txt)|*.txt|Документы (*.doc;*.docx;*.pdf)|*.doc;*.docx;*.pdf|Изображения (*.jpg;*.png;*.gif)|*.jpg;*.png;*.gif";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                selectedFilePath = dialog.FileName;
                FileNameTextBlock.Text = Path.GetFileName(selectedFilePath);
                UpdatePreview();
            }
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                MessageBox.Show("Сначала выберите файл!", "Внимание",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var saveDialog = new SaveFileDialog();
            saveDialog.Title = "Выберите формат для сохранения";
            saveDialog.Filter = "Текстовый файл (*.txt)|*.txt|" +
                               "PDF документ (*.pdf)|*.pdf|" +
                               "Документ Word (*.docx)|*.docx|" +
                               "Изображение JPEG (*.jpg)|*.jpg|" +
                               "Изображение PNG (*.png)|*.png|" +
                               "Все файлы (*.*)|*.*";

            bool? result = saveDialog.ShowDialog();

            if (result == true)
            {
                string savePath = saveDialog.FileName;
                string selectedFormat = Path.GetExtension(savePath).ToLower();

                try
                {
                    File.Copy(selectedFilePath, savePath, overwrite: true);

                    MessageBox.Show($"Файл успешно сохранен как: {Path.GetFileName(savePath)}\n" +
                                  $"Путь: {savePath}",
                                  "Успех",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении файла:\n{ex.Message}",
                                  "Ошибка",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                }
            }
        }

        private void UpdatePreview()
        {
            string previewText = "";

            if (!string.IsNullOrEmpty(selectedFolderPath))
            {
                previewText += $"Каталог: {selectedFolderPath}\n";

                try
                {
                    var files = Directory.GetFiles(selectedFolderPath);
                    var directories = Directory.GetDirectories(selectedFolderPath);
                    previewText += $"Файлов: {files.Length}, Папок: {directories.Length}\n\n";
                }
                catch
                {
                    previewText += "Не удалось прочитать содержимое каталога\n\n";
                }
            }

            if (!string.IsNullOrEmpty(selectedFilePath))
            {
                FileInfo fileInfo = new FileInfo(selectedFilePath);
                previewText += $"Файл: {Path.GetFileName(selectedFilePath)}\n";
                previewText += $"Размер: {fileInfo.Length / 1024.0:F2} KB\n";
                previewText += $"Изменен: {fileInfo.LastWriteTime:dd.MM.yyyy HH:mm}\n";
                previewText += $"Расширение: {Path.GetExtension(selectedFilePath)}";
            }

            if (string.IsNullOrEmpty(previewText))
            {
                previewText = "Выберите каталог и/или файл для отображения информации...";
            }

            PreviewTextBlock.Text = previewText;
        }
    }
}