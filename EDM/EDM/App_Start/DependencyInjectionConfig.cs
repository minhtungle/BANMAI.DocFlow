using Applications.QuanLyCoCauToChuc.Interfaces;
using Applications.QuanLyCoCauToChuc.Services;
using Applications.QuanLyDonVi.Interfaces;
using Applications.QuanLyDonVi.Services;
using Applications.QuanLyKieuNguoiDung.Interfaces;
using Applications.QuanLyKieuNguoiDung.Services;
using Applications.QuanLyNguoiDung.Interfaces;
using Applications.QuanLyNguoiDung.Services;
using Applications.QuanLyNhaCungCap.Interfaces;
using Applications.QuanLyNhaCungCap.Services;
using Autofac;
using Autofac.Integration.Mvc;
using EDM_DB;
using Infrastructure.Caching;
using Infrastructure.Helpers;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Public.AppServices;
using Public.Interfaces;
using QuanLyCoCauToChuc.Controllers;
using QuanLyDonVi.Controllers;
using QuanLyKieuNguoiDung.Controllers;
using QuanLyNguoiDung.Controllers;
using QuanLyNhaCungCap.Controllers;
using System;
using System.Data.Entity;
using System.Net.Http;
using System.Web.Mvc;

namespace EDM.App_Start
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            #region ✅ Đăng ký các Controller MVC
            builder.RegisterControllers(typeof(QuanLyDonViController).Assembly);
            builder.RegisterControllers(typeof(QuanLyNguoiDungController).Assembly);
            builder.RegisterControllers(typeof(QuanLyKieuNguoiDungController).Assembly);
            builder.RegisterControllers(typeof(QuanLyCoCauToChucController).Assembly);

            builder.RegisterControllers(typeof(QuanLyNhaCungCapController).Assembly);
            #endregion

            #region Đăng ký Infrastructure
            // ✅ Đăng ký DbContext (EF Designer with EDMX)
            builder.RegisterType<EDM_DBEntities>()
                   .As<DbContext>()
                   .InstancePerRequest(); // hoặc InstancePerLifetimeScope()

            // Đăng ký UserContext để Autofac biết cách tạo IUserContext
            builder.RegisterType<UserContext>()
                   .As<IUserContext>()
                   .InstancePerRequest();

            // ✅ Đăng ký UnitOfWork
            builder.RegisterType<EfUnitOfWork>()
                   .As<IUnitOfWork>()
                   .InstancePerRequest();

            // ✅ Đăng ký Generic Repository
            builder.RegisterGeneric(typeof(EfRepository<,>))
                   .As(typeof(IRepository<,>))
                   .InstancePerRequest();

            // ✅ Đăng ký Cache Manager
            builder.RegisterType<MemoryCacheManager>()
                   .As<ICacheManager>()
                   .SingleInstance(); // hoặc InstancePerRequest nếu cần
            #endregion


            #region ✅ Đăng ký AppServices
            builder.RegisterType<HttpClient>()
                   .As<HttpClient>()
                   .SingleInstance();

            builder.RegisterType<ViewRenderer>()
                   .As<IViewRenderer>()
                   .SingleInstance(); // hoặc InstancePerRequest() nếu cần theo scope

            // Đăng ký PermissionCheckerAppService
            builder.RegisterType<PermissionCheckerAppService>()
                   .As<IPermissionCheckerAppService>()
                   .InstancePerRequest(); // hoặc InstancePerLifetimeScope()
            builder.RegisterType<QuanLyDonViService>()
                  .As<IQuanLyDonViService>()
                  .InstancePerRequest();
            builder.RegisterType<QuanLyNguoiDungService>()
                   .As<IQuanLyNguoiDungService>()
                   .InstancePerRequest();
            builder.RegisterType<QuanLyKieuNguoiDungService>()
                   .As<IQuanLyKieuNguoiDungService>()
                   .InstancePerRequest();
            builder.RegisterType<QuanLyCoCauToChucService>()
                   .As<IQuanLyCoCauToChucService>()
                   .InstancePerRequest();


            builder.RegisterType<QuanLyNhaCungCapService>()
                   .As<IQuanLyNhaCungCapService>()
                   .InstancePerRequest();
            //builder.RegisterType<QuanLyTruongHocService>()
            //       .As<IQuanLyTruongHocService>()
            //       .InstancePerRequest();
            #endregion

            #region Đăng ký IRepositories
            builder.RegisterType<EfRepository<tbNguoiDung, Guid>>()
                   .As<IRepository<tbNguoiDung, Guid>>()
                   .InstancePerRequest();
            builder.RegisterType<EfRepository<tbNguoiDungKieuNguoiDung, Guid>>()
              .As<IRepository<tbNguoiDungKieuNguoiDung, Guid>>()
              .InstancePerRequest();
            builder.RegisterType<EfRepository<tbKieuNguoiDung, Guid>>()
                   .As<IRepository<tbKieuNguoiDung, Guid>>()
                   .InstancePerRequest();
            builder.RegisterType<EfRepository<tbCoCauToChuc, Guid>>()
                   .As<IRepository<tbCoCauToChuc, Guid>>()
                   .InstancePerRequest();
            builder.RegisterType<EfRepository<tbDonViSuDung, Guid>>()
                   .As<IRepository<tbDonViSuDung, Guid>>()
                   .InstancePerRequest();

            builder.RegisterType<EfRepository<tbNhaCungCap, Guid>>()
                   .As<IRepository<tbNhaCungCap, Guid>>()
                   .InstancePerRequest();
            builder.RegisterType<EfRepository<tbNhaCungCapTruongHoc, Guid>>()
                   .As<IRepository<tbNhaCungCapTruongHoc, Guid>>()
                   .InstancePerRequest();
            builder.RegisterType<EfRepository<tbTruongHoc, Guid>>()
                   .As<IRepository<tbTruongHoc, Guid>>()
                   .InstancePerRequest();
            builder.RegisterType<EfRepository<tbTaiLieuNhaCungCap, Guid>>()
                   .As<IRepository<tbTaiLieuNhaCungCap, Guid>>()
                   .InstancePerRequest();

            #endregion

            // 🔨 Build container
            var container = builder.Build();

            // ✅ Gán Autofac làm Dependency Resolver cho MVC
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}