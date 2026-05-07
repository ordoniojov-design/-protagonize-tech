FROM node:20-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm install && npm install --package-lock-only

COPY . .
RUN npm run build -- --configuration production

FROM nginx:alpine AS runtime
COPY --from=build /app/dist/task-manager-frontend/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80