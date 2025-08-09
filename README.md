# MVC-NextJS Hybrid Web Application

A modern full-stack web application that seamlessly integrates **ASP.NET MVC** backend with a **Next.js** frontend, combining the power of server-side C# with React's modern UI capabilities.

## 🏗️ Architecture

This project demonstrates a hybrid architecture approach:

- **Backend**: ASP.NET MVC (.NET 9) with C# controllers and server-side logic
- **Frontend**: Next.js 15 with React 19 and TypeScript for modern UI components
- **Integration**: YARP Reverse Proxy for seamless routing between backend and frontend
- **Development**: SPA Proxy integration for hot-reload development experience

## 🚀 Key Features

- **Hybrid Routing**: Combines MVC controller routes with Next.js page routing
- **TypeScript Support**: Full TypeScript integration for type-safe development
- **Modern React**: Built with React 19 and Next.js 15 for optimal performance
- **Development Experience**: Hot-reload and proxy configuration for seamless development
- **Production Ready**: Configured with error handling, HTTPS, and static file serving

## 🛠️ Technology Stack

### Backend
- .NET 9.0
- ASP.NET MVC
- YARP Reverse Proxy

### Frontend
- Next.js 15
- React 19
- TypeScript 5
- ESLint for code quality

## 📁 Project Structure

```
mvc-nextjs/
├── Webapp.NextJs/              # ASP.NET MVC application
│   ├── ClientApp/              # Next.js frontend application
│   │   ├── src/pages/          # Next.js pages
│   │   ├── package.json        # Frontend dependencies
│   │   └── next.config.ts      # Next.js configuration
│   ├── Program.cs              # ASP.NET application entry point
│   ├── ProxyConfigProvider.cs  # YARP proxy configuration
│   └── Webapp.NextJs.csproj    # .NET project file
└── README.md                   # This file
```

## 🚦 Getting Started

1. **Prerequisites**: Ensure you have .NET 9 SDK and Node.js installed
2. **Backend**: Run the ASP.NET application from `Webapp.NextJs/`
3. **Frontend**: The Next.js app will automatically start via SPA proxy integration
4. **Development**: Navigate to the configured localhost URL to see the integrated application

This architecture provides the flexibility of modern frontend frameworks while maintaining the robustness and familiarity of ASP.NET MVC for backend operations.
