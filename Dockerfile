# ---------- BUILD STAGE ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

ARG SUPABASE_URL
ARG SUPABASE_ANON_KEY
ARG OPENWEATHER_API_KEY

COPY . .

RUN echo "{ \
  \"Supabase\": { \
    \"Url\": \"${SUPABASE_URL}\", \
    \"AnonKey\": \"${SUPABASE_ANON_KEY}\" \
  }, \
  \"OpenWeather\": { \
    \"ApiKey\": \"${OPENWEATHER_API_KEY}\" \
  } \
}" > wwwroot/appsettings.json

RUN dotnet publish WeatherApp.csproj -c Release -o out

# ---------- RUNTIME STAGE ----------
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

COPY --from=build /app/out/wwwroot .

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
