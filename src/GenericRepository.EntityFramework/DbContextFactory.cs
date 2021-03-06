﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericRepository.EntityFramework
{
    public interface IDbContextFactory
    {
        T CreateDbContext<T>(String nameOrConnectionString) where T : DbContext, new();
    }
    public class DbContextFactory : IDbContextFactory
    {

        #region Implementation of IDbContextFactory

        public T CreateDbContext<T>(String nameOrConnectionString) where T : DbContext, new()
        {
            // Create a new instance of T and return.
            return new T();
        }

        #endregion
    }
}
