# Talentech Webhooks

Talentech supports webhooks to enable event-driven integrations between our SaaS platform and your systems. Webhooks allow you to receive real-time notifications when specific events occur — such as a candidate applying, a status update, or a new job posting.

This repository contains a **minimal and functional example** of a webhook receiver implemented in **.NET and C#**. It demonstrates how to:

- Listen for incoming HTTP POST requests
- Read and process the JSON payload
- Validate the authenticity of the webhook using HMAC-SHA256
- Respond appropriately to valid and invalid requests

## 🔐 Signature Validation

Each webhook request includes an `X-Signature-Base64HmacSha256` HTTP header. This is a Base64-encoded HMAC-SHA256 hash of the request body, computed using a shared secret. To ensure the webhook was sent by Talentech and was not tampered with, you must verify this signature.

### Signature Verification Steps

1. Retrieve the raw request body as a string.
2. Compute the HMAC-SHA256 hash using your shared secret and the body.
3. Base64-encode the hash.
4. Compare the result to the `X-Signature-Base64HmacSha256` header using a time-safe comparison.

