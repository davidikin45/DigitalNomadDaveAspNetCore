﻿// <auto-generated />
using System;
using DND.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DND.Data.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    partial class ApplicationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.3-rtm-32065")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DND.Domain.Blog.Authors.Author", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateModified");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("UrlSlug")
                        .HasMaxLength(50);

                    b.Property<string>("UserCreated");

                    b.Property<string>("UserModified");

                    b.Property<string>("UserOwner");

                    b.HasKey("Id");

                    b.ToTable("Author");
                });

            modelBuilder.Entity("DND.Domain.Blog.BlogPosts.BlogPost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Album")
                        .IsRequired();

                    b.Property<int>("AuthorId");

                    b.Property<string>("CarouselImage");

                    b.Property<string>("CarouselText")
                        .HasMaxLength(200);

                    b.Property<int>("CategoryId");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateModified");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(300000);

                    b.Property<int>("MapHeight");

                    b.Property<int>("MapZoom");

                    b.Property<bool>("Published");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("ShortDescription")
                        .IsRequired()
                        .HasMaxLength(5000);

                    b.Property<bool>("ShowInCarousel");

                    b.Property<bool>("ShowLocationDetail");

                    b.Property<bool>("ShowLocationMap");

                    b.Property<bool>("ShowPhotosInAlbum");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<string>("UrlSlug")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("UserCreated");

                    b.Property<string>("UserModified");

                    b.Property<string>("UserOwner");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("CategoryId");

                    b.ToTable("BlogPost");
                });

            modelBuilder.Entity("DND.Domain.Blog.BlogPosts.BlogPostLocation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BlogPostId");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateModified");

                    b.Property<int>("LocationId");

                    b.Property<string>("UserCreated");

                    b.Property<string>("UserModified");

                    b.Property<string>("UserOwner");

                    b.HasKey("Id");

                    b.HasIndex("BlogPostId");

                    b.HasIndex("LocationId");

                    b.ToTable("BlogPostLocation");
                });

            modelBuilder.Entity("DND.Domain.Blog.BlogPosts.BlogPostTag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BlogPostId");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateModified");

                    b.Property<int>("TagId");

                    b.Property<string>("UserCreated");

                    b.Property<string>("UserModified");

                    b.Property<string>("UserOwner");

                    b.HasKey("Id");

                    b.HasIndex("BlogPostId");

                    b.HasIndex("TagId");

                    b.ToTable("BlogPostTag");
                });

            modelBuilder.Entity("DND.Domain.Blog.Categories.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateModified");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<bool>("Published");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("UrlSlug")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("UserCreated");

                    b.Property<string>("UserModified");

                    b.Property<string>("UserOwner");

                    b.HasKey("Id");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("DND.Domain.Blog.Locations.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Album");

                    b.Property<string>("Cost");

                    b.Property<bool>("CurrentLocation");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateModified");

                    b.Property<string>("DescriptionBody");

                    b.Property<double?>("Latitude");

                    b.Property<string>("Link");

                    b.Property<string>("LinkText");

                    b.Property<string>("LocationType")
                        .IsRequired();

                    b.Property<double?>("Longitude");

                    b.Property<string>("Name");

                    b.Property<bool>("NextLocation");

                    b.Property<string>("PlaceId");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<bool>("ShowOnTravelMap");

                    b.Property<string>("TimeRequired");

                    b.Property<string>("UrlSlug");

                    b.Property<string>("UserCreated");

                    b.Property<string>("UserModified");

                    b.Property<string>("UserOwner");

                    b.HasKey("Id");

                    b.ToTable("Location");
                });

            modelBuilder.Entity("DND.Domain.Blog.Tags.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateModified");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("UrlSlug")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("UserCreated");

                    b.Property<string>("UserModified");

                    b.Property<string>("UserOwner");

                    b.HasKey("Id");

                    b.ToTable("Tag");
                });

            modelBuilder.Entity("DND.Domain.CMS.CarouselItems.CarouselItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Album");

                    b.Property<string>("ButtonText");

                    b.Property<string>("CarouselText")
                        .HasMaxLength(200);

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateModified");

                    b.Property<string>("Image");

                    b.Property<string>("Link");

                    b.Property<bool>("OpenInNewWindow");

                    b.Property<bool>("Published");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Title");

                    b.Property<string>("UserCreated");

                    b.Property<string>("UserModified");

                    b.Property<string>("UserOwner");

                    b.HasKey("Id");

                    b.ToTable("CarouselItem");
                });

            modelBuilder.Entity("DND.Domain.CMS.ContentHtmls.ContentHtml", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateModified");

                    b.Property<string>("HTML");

                    b.Property<bool>("PreventDelete");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("UserCreated");

                    b.Property<string>("UserModified");

                    b.Property<string>("UserOwner");

                    b.HasKey("Id");

                    b.ToTable("ContentHtml");
                });

            modelBuilder.Entity("DND.Domain.CMS.ContentTexts.ContentText", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateModified");

                    b.Property<bool>("PreventDelete");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Text");

                    b.Property<string>("UserCreated");

                    b.Property<string>("UserModified");

                    b.Property<string>("UserOwner");

                    b.HasKey("Id");

                    b.ToTable("ContentText");
                });

            modelBuilder.Entity("DND.Domain.CMS.Faqs.Faq", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Answer")
                        .IsRequired();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateModified");

                    b.Property<string>("Question")
                        .IsRequired();

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("UserCreated");

                    b.Property<string>("UserModified");

                    b.Property<string>("UserOwner");

                    b.HasKey("Id");

                    b.ToTable("Faq");
                });

            modelBuilder.Entity("DND.Domain.CMS.MailingLists.MailingList", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateModified");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("Name");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("UserCreated");

                    b.Property<string>("UserModified");

                    b.Property<string>("UserOwner");

                    b.HasKey("Id");

                    b.ToTable("MailingList");
                });

            modelBuilder.Entity("DND.Domain.CMS.Projects.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Album");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateModified");

                    b.Property<string>("DescriptionText")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("File");

                    b.Property<string>("Link");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("UserCreated");

                    b.Property<string>("UserModified");

                    b.Property<string>("UserOwner");

                    b.HasKey("Id");

                    b.ToTable("Project");
                });

            modelBuilder.Entity("DND.Domain.CMS.Testimonials.Testimonial", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateModified");

                    b.Property<string>("File");

                    b.Property<string>("QuoteText")
                        .IsRequired()
                        .HasMaxLength(5000);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("UserCreated");

                    b.Property<string>("UserModified");

                    b.Property<string>("UserOwner");

                    b.HasKey("Id");

                    b.ToTable("Testimonial");
                });

            modelBuilder.Entity("DND.Domain.Blog.BlogPosts.BlogPost", b =>
                {
                    b.HasOne("DND.Domain.Blog.Authors.Author", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DND.Domain.Blog.Categories.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("DND.Domain.Blog.BlogPosts.BlogPostLocation", b =>
                {
                    b.HasOne("DND.Domain.Blog.BlogPosts.BlogPost")
                        .WithMany("Locations")
                        .HasForeignKey("BlogPostId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DND.Domain.Blog.Locations.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DND.Domain.Blog.BlogPosts.BlogPostTag", b =>
                {
                    b.HasOne("DND.Domain.Blog.BlogPosts.BlogPost")
                        .WithMany("Tags")
                        .HasForeignKey("BlogPostId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DND.Domain.Blog.Tags.Tag", "Tag")
                        .WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}