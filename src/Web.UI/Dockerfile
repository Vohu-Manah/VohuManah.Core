# Build stage
FROM node:20-alpine AS build
WORKDIR /app

ARG VITE_API_BASE_URL=http://web-api:5000
ENV VITE_API_BASE_URL=${VITE_API_BASE_URL}

COPY package.json package-lock.json ./
RUN npm install --legacy-peer-deps

COPY . .
RUN npm run build

# Serve stage
FROM nginx:alpine AS serve
WORKDIR /usr/share/nginx/html
COPY --from=build /app/dist ./
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

# Use the official Node.js version from https://hub.docker.com/
FROM node:22.20.0-alpine

# Set the working directory inside the container to /app
WORKDIR /app

# Copy only the package.json file to the working directory
COPY package.json .

# Install npm dependencies based on the package.json
RUN npm install --legacy-peer-deps

# Copy the rest of the application code to the working directory
COPY . .

# Expose port 5002 to allow external access
EXPOSE 5002

# Specify the command to run the application when the container starts
CMD [ "npm", "run", "dev"]
