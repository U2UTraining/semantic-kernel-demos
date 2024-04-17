using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2SQL.Infra;
public static class TableProvider
{
    private static List<string> tableDefinitions = 
    [
        """
            CREATE TABLE [dbo].[Products](
        [ProductID] [int] IDENTITY(1,1) NOT NULL,
        [ProductName] [nvarchar](40) NOT NULL,
        [SupplierID] [int] NULL,
        [CategoryID] [int] NULL,
        [QuantityPerUnit] [nvarchar](20) NULL,
        [UnitPrice] [money] NULL,
        [UnitsInStock] [smallint] NULL,
        [UnitsOnOrder] [smallint] NULL,
        [ReorderLevel] [smallint] NULL,
        [Discontinued] [bit] NOT NULL,
         CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
        (
        	[ProductID] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
        ) ON [PRIMARY]
        """,
        """
            CREATE TABLE [dbo].[Categories](
        [CategoryID] [int] IDENTITY(1,1) NOT NULL,
        [CategoryName] [nvarchar](15) NOT NULL,
        [Description] [ntext] NULL,
        [Picture] [image] NULL,
         CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
        (
        	[CategoryID] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
        """,
        """
            CREATE TABLE [dbo].[Suppliers](
        [SupplierID] [int] IDENTITY(1,1) NOT NULL,
        [CompanyName] [nvarchar](40) NOT NULL,
        [ContactName] [nvarchar](30) NULL,
        [ContactTitle] [nvarchar](30) NULL,
        [Address] [nvarchar](60) NULL,
        [City] [nvarchar](15) NULL,
        [Region] [nvarchar](15) NULL,
        [PostalCode] [nvarchar](10) NULL,
        [Country] [nvarchar](15) NULL,
        [Phone] [nvarchar](24) NULL,
        [Fax] [nvarchar](24) NULL,
        [HomePage] [ntext] NULL,
         CONSTRAINT [PK_Suppliers] PRIMARY KEY CLUSTERED 
        (
        	[SupplierID] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
        """,
        """
            CREATE TABLE [dbo].[Customers](
        [CustomerID] [nchar](5) NOT NULL,
        [CompanyName] [nvarchar](40) NOT NULL,
        [ContactName] [nvarchar](30) NULL,
        [ContactTitle] [nvarchar](30) NULL,
        [Address] [nvarchar](60) NULL,
        [City] [nvarchar](15) NULL,
        [Region] [nvarchar](15) NULL,
        [PostalCode] [nvarchar](10) NULL,
        [Country] [nvarchar](15) NULL,
        [Phone] [nvarchar](24) NULL,
        [Fax] [nvarchar](24) NULL,
         CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
        (
        	[CustomerID] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
        ) ON [PRIMARY]
        """,
        """
            CREATE TABLE [dbo].[Orders](
        [OrderID] [int] IDENTITY(1,1) NOT NULL,
        [CustomerID] [nchar](5) NULL,
        [EmployeeID] [int] NULL,
        [OrderDate] [datetime] NULL,
        [RequiredDate] [datetime] NULL,
        [ShippedDate] [datetime] NULL,
        [ShipVia] [int] NULL,
        [Freight] [money] NULL,
        [ShipName] [nvarchar](40) NULL,
        [ShipAddress] [nvarchar](60) NULL,
        [ShipCity] [nvarchar](15) NULL,
        [ShipRegion] [nvarchar](15) NULL,
        [ShipPostalCode] [nvarchar](10) NULL,
        [ShipCountry] [nvarchar](15) NULL,
         CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
        (
        	[OrderID] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
        ) ON [PRIMARY]
        """,
        """
            CREATE TABLE [dbo].[Order Details](
        [OrderID] [int] NOT NULL,
        [ProductID] [int] NOT NULL,
        [UnitPrice] [money] NOT NULL,
        [Quantity] [smallint] NOT NULL,
        [Discount] [real] NOT NULL,
         CONSTRAINT [PK_Order_Details] PRIMARY KEY CLUSTERED 
        (
        	[OrderID] ASC,
        	[ProductID] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
        ) ON [PRIMARY]
        """
    ];

    public static List<string> GetTableDefinitions()
    {
        return tableDefinitions;
    }
}
