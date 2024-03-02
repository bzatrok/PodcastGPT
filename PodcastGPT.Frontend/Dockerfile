# Base image
FROM node:18-alpine AS base
WORKDIR /app
COPY package.json yarn.lock* ./

# Dependencies
FROM base AS dependencies
RUN yarn install --frozen-lockfile && \
    yarn next telemetry disable

# Build
FROM dependencies AS builder
COPY . .

ENV NEXT_PUBLIC_FEATURE_FLAG_GA4_ANALYTICS="true" \
    NEXT_PUBLIC_FEATURE_FLAG_MIX_MATCH_BOOSTING="true" \
    NEXT_PUBLIC_FEATURE_FLAG_DATASET_EDITOR="false" \
    NEXT_PUBLIC_FEATURE_FLAG_SYNONYM_EDITOR="true" \
    NEXT_PUBLIC_FEATURE_FLAG_REDIRECT_EDITOR="false"

# Removing unnecessary files and combining environment variables
RUN rm -rf .env .env.local .env.local.template && \
    yarn build && \
    rm -rf node_modules

# Runner
FROM node:18-alpine AS runner
WORKDIR /app

ENV NODE_ENV production

COPY --from=builder /app/next.config.js ./
COPY --from=builder /app/.next ./.next
# COPY --from=builder /app/node_modules ./node_modules
COPY --from=builder /app/package.json ./package.json
COPY --from=builder /app/public ./public

# Installing only production dependencies
RUN yarn install --production

EXPOSE 3000

CMD yarn start