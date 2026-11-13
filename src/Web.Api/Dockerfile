FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
RUN apt-get update && apt-get install -y --no-install-recommends fontconfig fonts-farsiweb libfreetype6 libx11-6 libxext6 libxrender1 libxcb1 libxcb-render0 libxcb-shape0 libxcb-xfixes0 libxcb-shm0 libxcb-util1 libgdiplus \
    && ln -sf /usr/lib/libgdiplus.so /usr/lib/libgdiplus \
    && ln -sf /usr/lib/libgdiplus.so /usr/lib/x86_64-linux-gnu/libgdiplus.dll.so \
    && ln -sf /usr/lib/libgdiplus.so /usr/lib/x86_64-linux-gnu/gdiplus.dll.so \
    && ln -sf /usr/lib/libgdiplus.so /app/libgdiplus.dll.so \
    && ln -sf /usr/lib/libgdiplus.so /app/gdiplus.dll.so \
    && mkdir -p /app/runtimes/linux-x64/native \
    && ln -sf /usr/lib/libgdiplus.so /app/runtimes/linux-x64/native/libgdiplus.dll.so \
    && ln -sf /usr/lib/libgdiplus.so /app/runtimes/linux-x64/native/gdiplus.dll.so \
    && ldconfig \
    && rm -rf /var/lib/apt/lists/*

# کپی فونتهای Shabnam پروژه به مسیر فونتهای لینوکس
COPY src/Infrastructure/Reports/Html/Resources/fonts/*.ttf /usr/share/fonts/truetype/
COPY docker/deploy/fonts/ /opt/vohumanah-fonts/
COPY docker/deploy/entrypoint-fonts.sh /opt/entrypoint-fonts.sh
RUN fc-cache -fv && sed -i 's/\r$//' /opt/entrypoint-fonts.sh && chmod +x /opt/entrypoint-fonts.sh

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["src/Web.Api/Web.Api.csproj", "src/Web.Api/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/SharedKernel/SharedKernel.csproj", "src/SharedKernel/"]
RUN dotnet restore "./src/Web.Api/Web.Api.csproj"
COPY . .
WORKDIR "/src/src/Web.Api"
RUN dotnet build "./Web.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Web.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["/opt/entrypoint-fonts.sh"]
