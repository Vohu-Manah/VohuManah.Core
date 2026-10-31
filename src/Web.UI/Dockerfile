FROM node:20-alpine AS build
WORKDIR /app

# Empty default => relative API URLs routed same-origin through nginx (nginx.conf).
ENV VITE_API_BASE_URL=${VITE_API_BASE_URL}

COPY package.json package-lock.json ./
RUN npm install --legacy-peer-deps

COPY . .
RUN npm run build

FROM nginx:alpine AS serve
WORKDIR /usr/share/nginx/html
COPY --from=build /app/dist ./
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 5002
CMD ["nginx", "-g", "daemon off;"]
