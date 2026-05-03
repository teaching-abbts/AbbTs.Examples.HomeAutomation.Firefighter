<template>
  <TresPerspectiveCamera
    make-default
    :position="[0, 18, 20]"
    :fov="45"
    :look-at="[0, 0, 0]"
  />
  <OrbitControls
    :target="[0, 0, 0]"
    :enable-damping="true"
    :enable-pan="false"
    :auto-rotate="true"
    :auto-rotate-speed="2"
    :min-distance="8"
    :max-distance="35"
    :max-polar-angle="Math.PI / 2.1"
  />
</template>

<script lang="ts" setup>
import { OrbitControls } from "@tresjs/cientos";
</script>
