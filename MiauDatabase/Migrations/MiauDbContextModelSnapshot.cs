﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace MiauDatabase.Migrations
{
    [DbContext(typeof(MiauDbContext))]
    partial class MiauDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.7");

            modelBuilder.Entity("MiauDatabase.Entities.AddressEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("TEXT")
                        .HasColumnName("address");

                    b.Property<string>("Cep")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("TEXT")
                        .HasColumnName("cep");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("TEXT")
                        .HasColumnName("city");

                    b.Property<string>("Complement")
                        .HasMaxLength(15)
                        .HasColumnType("TEXT")
                        .HasColumnName("complement");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("TEXT")
                        .HasColumnName("date_added");

                    b.Property<string>("Destinatary")
                        .HasMaxLength(60)
                        .HasColumnType("TEXT")
                        .HasColumnName("destinatary");

                    b.Property<string>("Neighborhood")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("TEXT")
                        .HasColumnName("neighborhood");

                    b.Property<int>("Number")
                        .HasColumnType("INTEGER")
                        .HasColumnName("number");

                    b.Property<string>("Reference")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT")
                        .HasColumnName("reference");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("TEXT")
                        .HasColumnName("state");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_addresses");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_addresses_user_id");

                    b.ToTable("addresses", (string)null);

                    b.HasComment("Represents an address.");
                });

            modelBuilder.Entity("MiauDatabase.Entities.AppointmentEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("TEXT")
                        .HasColumnName("date_added");

                    b.Property<int>("PetId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("pet_id");

                    b.Property<decimal>("Price")
                        .HasColumnType("TEXT")
                        .HasColumnName("price");

                    b.Property<DateTime>("ScheduledTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("scheduled_time");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER")
                        .HasColumnName("type");

                    b.HasKey("Id")
                        .HasName("pk_appointments");

                    b.HasIndex("PetId")
                        .HasDatabaseName("ix_appointments_pet_id");

                    b.ToTable("appointments", (string)null);

                    b.HasComment("Represents an appointment.");
                });

            modelBuilder.Entity("MiauDatabase.Entities.CouponEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("Coupon")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("TEXT")
                        .HasColumnName("coupon");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("TEXT")
                        .HasColumnName("date_added");

                    b.Property<decimal>("Discount")
                        .HasColumnType("TEXT")
                        .HasColumnName("discount");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER")
                        .HasColumnName("is_active");

                    b.HasKey("Id")
                        .HasName("pk_coupons");

                    b.ToTable("coupons", (string)null);

                    b.HasComment("Represents a discount coupon.");
                });

            modelBuilder.Entity("MiauDatabase.Entities.PetEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("Breed")
                        .HasMaxLength(30)
                        .HasColumnType("TEXT")
                        .HasColumnName("breed");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("TEXT")
                        .HasColumnName("date_added");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("TEXT")
                        .HasColumnName("date_of_birth");

                    b.Property<int>("Gender")
                        .HasColumnType("INTEGER")
                        .HasColumnName("gender");

                    b.Property<string>("ImageFileUrl")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT")
                        .HasColumnName("image_file_url");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER")
                        .HasColumnName("type");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_pets");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_pets_user_id");

                    b.ToTable("pets", (string)null);

                    b.HasComment("Represents a pet.");
                });

            modelBuilder.Entity("MiauDatabase.Entities.ProductEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<int>("Amount")
                        .HasColumnType("INTEGER")
                        .HasColumnName("amount");

                    b.Property<string>("Brand")
                        .HasMaxLength(30)
                        .HasColumnType("TEXT")
                        .HasColumnName("brand");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("TEXT")
                        .HasColumnName("date_added");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("TEXT")
                        .HasColumnName("description");

                    b.Property<decimal>("Discount")
                        .HasColumnType("TEXT")
                        .HasColumnName("discount");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER")
                        .HasColumnName("is_active");

                    b.Property<decimal>("Price")
                        .HasColumnType("TEXT")
                        .HasColumnName("price");

                    b.Property<int>("Tags")
                        .HasColumnType("INTEGER")
                        .HasColumnName("tags");

                    b.HasKey("Id")
                        .HasName("pk_products");

                    b.ToTable("products", (string)null);

                    b.HasComment("Represents a store product.");
                });

            modelBuilder.Entity("MiauDatabase.Entities.ProductImageEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("TEXT")
                        .HasColumnName("date_added");

                    b.Property<string>("FileUrl")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("file_url");

                    b.Property<int>("ProductId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("product_id");

                    b.HasKey("Id")
                        .HasName("pk_product_images");

                    b.HasIndex("ProductId")
                        .HasDatabaseName("ix_product_images_product_id");

                    b.ToTable("product_images", (string)null);

                    b.HasComment("Represents a product image.");
                });

            modelBuilder.Entity("MiauDatabase.Entities.ProductReviewEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("TEXT")
                        .HasColumnName("date_added");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("description");

                    b.Property<int>("ProductId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("product_id");

                    b.Property<int>("Score")
                        .HasColumnType("INTEGER")
                        .HasColumnName("score");

                    b.Property<int?>("UserId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_product_reviews");

                    b.HasIndex("ProductId")
                        .HasDatabaseName("ix_product_reviews_product_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_product_reviews_user_id");

                    b.ToTable("product_reviews", (string)null);

                    b.HasComment("Represents a product image.");
                });

            modelBuilder.Entity("MiauDatabase.Entities.PurchasedProductEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("TEXT")
                        .HasColumnName("date_added");

                    b.Property<int>("ProductId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("product_id");

                    b.Property<int>("PurchaseId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("purchase_id");

                    b.Property<decimal>("SalePrice")
                        .HasColumnType("TEXT")
                        .HasColumnName("sale_price");

                    b.HasKey("Id")
                        .HasName("pk_purchased_products");

                    b.HasIndex("ProductId")
                        .HasDatabaseName("ix_purchased_products_product_id");

                    b.HasIndex("PurchaseId")
                        .HasDatabaseName("ix_purchased_products_purchase_id");

                    b.ToTable("purchased_products", (string)null);

                    b.HasComment("Represents a purchased product.");
                });

            modelBuilder.Entity("MiauDatabase.Entities.PurchaseEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<int?>("CouponId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("coupon_id");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("TEXT")
                        .HasColumnName("date_added");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER")
                        .HasColumnName("status");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_purchases");

                    b.HasIndex("CouponId")
                        .HasDatabaseName("ix_purchases_coupon_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_purchases_user_id");

                    b.ToTable("purchases", (string)null);

                    b.HasComment("Represents a purchase.");
                });

            modelBuilder.Entity("MiauDatabase.Entities.UserEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("Cpf")
                        .IsRequired()
                        .HasMaxLength(14)
                        .HasColumnType("TEXT")
                        .HasColumnName("cpf");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("TEXT")
                        .HasColumnName("date_added");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("TEXT")
                        .HasColumnName("email");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<int>("Permissions")
                        .HasColumnType("INTEGER")
                        .HasColumnName("permissions");

                    b.Property<string>("Phone")
                        .HasMaxLength(14)
                        .HasColumnType("TEXT")
                        .HasColumnName("phone");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT")
                        .HasColumnName("salt");

                    b.Property<string>("SaltedPassword")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT")
                        .HasColumnName("salted_password");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("TEXT")
                        .HasColumnName("surname");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.ToTable("users", (string)null);

                    b.HasComment("Represents a user.");
                });

            modelBuilder.Entity("MiauDatabase.Entities.WishlistEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("TEXT")
                        .HasColumnName("date_added");

                    b.Property<int>("ProductId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("product_id");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_wishlist");

                    b.HasIndex("ProductId")
                        .HasDatabaseName("ix_wishlist_product_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_wishlist_user_id");

                    b.ToTable("wishlist", (string)null);

                    b.HasComment("Represents a wishlist item.");
                });

            modelBuilder.Entity("MiauDatabase.Entities.AddressEntity", b =>
                {
                    b.HasOne("MiauDatabase.Entities.UserEntity", "User")
                        .WithMany("Addresses")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_addresses_users_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MiauDatabase.Entities.AppointmentEntity", b =>
                {
                    b.HasOne("MiauDatabase.Entities.PetEntity", "Pet")
                        .WithMany("Appointments")
                        .HasForeignKey("PetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_appointments_pets_pet_id");

                    b.Navigation("Pet");
                });

            modelBuilder.Entity("MiauDatabase.Entities.PetEntity", b =>
                {
                    b.HasOne("MiauDatabase.Entities.UserEntity", "User")
                        .WithMany("Pets")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_pets_users_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MiauDatabase.Entities.ProductImageEntity", b =>
                {
                    b.HasOne("MiauDatabase.Entities.ProductEntity", "Product")
                        .WithMany("ProductImages")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_product_images_products_product_id");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("MiauDatabase.Entities.ProductReviewEntity", b =>
                {
                    b.HasOne("MiauDatabase.Entities.ProductEntity", "Product")
                        .WithMany("ProductReviews")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_product_reviews_products_product_id");

                    b.HasOne("MiauDatabase.Entities.UserEntity", "User")
                        .WithMany("ProductReviews")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_product_reviews_users_user_id");

                    b.Navigation("Product");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MiauDatabase.Entities.PurchasedProductEntity", b =>
                {
                    b.HasOne("MiauDatabase.Entities.ProductEntity", "Product")
                        .WithMany("PurchasedProducts")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_purchased_products_products_product_id");

                    b.HasOne("MiauDatabase.Entities.PurchaseEntity", "Purchase")
                        .WithMany("PurchasedProduct")
                        .HasForeignKey("PurchaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_purchased_products_purchases_purchase_id");

                    b.Navigation("Product");

                    b.Navigation("Purchase");
                });

            modelBuilder.Entity("MiauDatabase.Entities.PurchaseEntity", b =>
                {
                    b.HasOne("MiauDatabase.Entities.CouponEntity", "Coupon")
                        .WithMany("Purchases")
                        .HasForeignKey("CouponId")
                        .HasConstraintName("fk_purchases_coupons_coupon_id");

                    b.HasOne("MiauDatabase.Entities.UserEntity", "User")
                        .WithMany("Purchases")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_purchases_users_user_id");

                    b.Navigation("Coupon");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MiauDatabase.Entities.WishlistEntity", b =>
                {
                    b.HasOne("MiauDatabase.Entities.ProductEntity", "Product")
                        .WithMany("Wishlist")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_wishlist_products_product_id");

                    b.HasOne("MiauDatabase.Entities.UserEntity", "User")
                        .WithMany("Wishlist")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_wishlist_users_user_id");

                    b.Navigation("Product");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MiauDatabase.Entities.CouponEntity", b =>
                {
                    b.Navigation("Purchases");
                });

            modelBuilder.Entity("MiauDatabase.Entities.PetEntity", b =>
                {
                    b.Navigation("Appointments");
                });

            modelBuilder.Entity("MiauDatabase.Entities.ProductEntity", b =>
                {
                    b.Navigation("ProductImages");

                    b.Navigation("ProductReviews");

                    b.Navigation("PurchasedProducts");

                    b.Navigation("Wishlist");
                });

            modelBuilder.Entity("MiauDatabase.Entities.PurchaseEntity", b =>
                {
                    b.Navigation("PurchasedProduct");
                });

            modelBuilder.Entity("MiauDatabase.Entities.UserEntity", b =>
                {
                    b.Navigation("Addresses");

                    b.Navigation("Pets");

                    b.Navigation("ProductReviews");

                    b.Navigation("Purchases");

                    b.Navigation("Wishlist");
                });
#pragma warning restore 612, 618
        }
    }
}
