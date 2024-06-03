<script setup lang="ts">
import { VueQueryDevtools } from '@tanstack/vue-query-devtools'

import Card from 'primevue/card'
import { RouterView } from 'vue-router'
import PageHeading from './components/PageHeading.vue'
</script>

<template>
  <div class="">
    <Card class="h-screen">
      <template #title>
        <PageHeading />
      </template>
      <template #content>
        <RouterView />
        <VueQueryDevtools />
      </template>
    </Card>
  </div>
</template>
