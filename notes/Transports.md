# Transports for Messages 

- Cloud Native Transports / Brokers
    - RabbitMQ - Message Queue, it does not support EventStreaming. 
    - Redis - (Redis is known as a cache, it has a feature called "RedisStreams" which is EventStreaming)
    - Kafka - Event Streaming - (Confluent) 
    - Database (not the greatest, but better than nothing)
    - NATS - https://nats.io/ (free, open source, cloud, support, fast, etc.)
- Proprietary
    - Amazon SQS - Queuing, Event Streaming, FAn out, etc.
    - Azure Service Bus
    - Azure Event Hubs (Kafka Clone)