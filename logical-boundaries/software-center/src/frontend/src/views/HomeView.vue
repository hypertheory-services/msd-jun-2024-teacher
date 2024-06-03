<script setup lang="ts">
import Card from 'primevue/card'
import TabView from 'primevue/tabview'
import TabPanel from 'primevue/tabpanel'
</script>

<template>
  <main>
    <Card>
      <template #title>
        <h1>The Software Center</h1>
      </template>
      <template #content>
        <p>
          This is a fictional software management tool for the class
          <em>Microservie Development</em> by <strong>Hypertheory Training</strong>.
        </p>
        <TabView>
          <TabPanel header="Backend Technology">
            <p class="m-0 mb-2">This is an ASP.NET Core API (.NET 8).</p>
            <p class="m-0 mb-2">
              It utilizes a backend-for-frontend pattern, this means the API is built by, delivered
              by, and tested by the same "team" that owns the frontend.
            </p>
            <p class="m-0 mb-2">
              It uses a Postgres database for storing it's data, and uses the
              <a href="https://martendb.io/" target="_blank">Marten</a> library for interacting with
              the database.
            </p>
            <p class="m-0 mb-2">
              In addition, it uses the
              <a href="https://wolverinefx.net/" target="_blank">Wolverine</a> Library for messaging
              to a set of <a href="https://kafka.apache.org/" target="_blank"> Kafka</a> topics.
            </p>
          </TabPanel>
          <TabPanel header="Frontend Technology">
            <p class="m-0 mb-2">
              The Frontend is a <a href="https://vuejs.org/" target="_blank"> Vue 3</a> application
              that uses the <a href="https://primevue.org/" target="_blank">PrimeVue</a> component
              library.
            </p>
            <p class="m-0 mb-2">
              It also uses the
              <a
                href="https://tanstack.com/query/latest/docs/framework/vue/overview"
                target="_blank"
              >
                Tanstack Vue Query</a
              >
              tool for API interactions.
            </p>
          </TabPanel>
        </TabView>
      </template>
    </Card>
  </main>
</template>
