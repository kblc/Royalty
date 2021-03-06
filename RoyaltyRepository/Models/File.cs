﻿using RoyaltyRepository.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyRepository.Models
{
    public partial class RepositoryContext
    {
        /// <summary>
        /// Хранилище файлов
        /// </summary>
        public DbSet<File> Files { get; set; }
    }

    /// <summary>
    /// Файл
    /// </summary>
    [Table("file")]
    public partial class File : EntityBase
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("file_uid")]
        public Guid FileUID { get; set; }
        /// <summary>
        /// Имя файла
        /// </summary>
        [Column("file_name"), Required]
        public string FileName { get; set; }
        /// <summary>
        /// Путь к файлу
        /// </summary>
        [Column("file_path"), Required]
        public string OriginalFileName { get; set; }
        /// <summary>
        /// Размер файла
        /// </summary>
        [Column("file_size"), Required]
        public long FileSize { get; set; }
        /// <summary>
        /// Mime-тип файла
        /// </summary>
        [Column("mime"), Required]
        public string MimeType { get; set; }
        /// <summary>
        /// Дата создания записи
        /// </summary>
        [Column("date"), Required]
        public DateTime Date { get; set; }
        /// <summary>
        /// Кодировка файла
        /// </summary>
        [Column("encoding"), MaxLength(100)]
        [Obsolete("Use Encoding property")]
        public string EncodingName { get; set; }
        /// <summary>
        /// Кодировка для текстовых файлов
        /// </summary>
        [NotMapped]
        public Encoding Encoding
        {
#pragma warning disable 618
            get { return Extensions.Extensions.GetEncodingByName(EncodingName); }
            set { EncodingName = value?.WebName; }
#pragma warning restore 618
        }
    }
}
