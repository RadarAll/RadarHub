<template>
        <v-navigation-drawer permanent :width="230" class="elevation-1">
                <div class="d-flex align-center justify-center w-full py-8">
                    <v-img :src="logo" :width="40" :height="40"/>
                </div>

                <v-divider class="mb-4"></v-divider>

                <div class="d-flex flex-column pa-3">
                        <a href=""class="d-flex flex-row px-3 text-decoration-none ga-3 w-full">
                            <v-icon color="green-primary">mdi-home-variant-outline</v-icon>
                            <span class="font-weight-medium text-green-primary text-no-wrap text-subtitle-2 overflow-hidden">Dashboard</span>
                        </a>
                </div>

                <div class="d-flex flex-column pa-3">
                        <a href=""class="d-flex flex-row px-3 text-decoration-none ga-3 w-full">
                            <v-icon color="green-primary">mdi-calendar-blank-outline</v-icon>
                            <span class="font-weight-medium text-green-primary text-no-wrap text-subtitle-2 overflow-hidden">Eventos</span>
                        </a>
                </div>

                <div class="d-flex flex-column pa-3">
                        <a href=""class="d-flex flex-row px-3 text-decoration-none ga-3 w-full">
                            <v-icon color="green-primary">mdi-account</v-icon>
                            <span class="font-weight-medium text-green-primary text-no-wrap text-subtitle-2 overflow-hidden">Usuarios</span>
                        </a>
                </div>

                <div class="d-flex flex-column pa-3">
                        <a href=""class="d-flex flex-row px-3 text-decoration-none ga-3 w-full">
                            <v-icon color="green-primary">mdi-gavel</v-icon>
                            <span class="font-weight-medium text-green-primary text-no-wrap text-subtitle-2 overflow-hidden">Licitações</span>
                        </a>
                </div>

                <div class="d-flex flex-column pa-3">
                        <a href=""class="d-flex flex-row px-3 text-decoration-none ga-3 w-full">
                            <v-icon color="green-primary">mdi-history</v-icon>
                            <span class="font-weight-medium text-green-primary text-no-wrap text-subtitle-2 overflow-hidden">Histórico</span>
                        </a>
                </div>
        </v-navigation-drawer>
</template>

<script setup>
import logo from '@/assets/logo.png'
</script>