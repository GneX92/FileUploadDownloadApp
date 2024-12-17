using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FileUploadDownload.Models;

public partial class NetdbContext : DbContext
{
    public NetdbContext()
    {
    }

    public NetdbContext( DbContextOptions<NetdbContext> options )
        : base( options )
    {
    }

    public virtual DbSet<DocStore> DocStore { get; set; }

    protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder )
        => optionsBuilder.UseLazyLoadingProxies()
                         .UseSqlServer( "Server=.\\SQLEXPRESS;Database=NETDB;Trusted_Connection=True;MultipleActiveResultSets=True;Encrypt=false;" );

    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        modelBuilder.Entity<DocStore>( entity =>
        {
            entity.HasKey( e => e.DocId )
                .HasName( "PK_DOC_STORE" )
                .IsClustered( false );

            entity.ToTable( "DocStore" );

            entity.HasIndex( e => e.DocName , "IDX_NAME" ).IsClustered();

            entity.Property( e => e.DocId ).HasColumnName( "DocID" );
            entity.Property( e => e.ContentType ).HasMaxLength( 100 );
            entity.Property( e => e.DocName )
                .HasMaxLength( 512 )
                .IsUnicode( false );
            entity.Property( e => e.InsertionDate )
                .HasDefaultValueSql( "(getdate())" )
                .HasColumnType( "datetime" );
        } );

        OnModelCreatingPartial( modelBuilder );
    }

    partial void OnModelCreatingPartial( ModelBuilder modelBuilder );
}