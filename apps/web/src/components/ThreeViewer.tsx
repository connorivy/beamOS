import { useEffect, useRef, useState } from "react";
import * as THREE from "three";

export const ThreeViewer = () => {
  const containerRef = useRef<HTMLDivElement | null>(null);
  const [renderError, setRenderError] = useState<string | null>(null);

  useEffect(() => {
    const container = containerRef.current;
    if (!container) {
      return;
    }

    try {
      const scene = new THREE.Scene();
      scene.background = new THREE.Color("#0d1117");

      const camera = new THREE.PerspectiveCamera(60, container.clientWidth / container.clientHeight, 0.1, 1000);
      camera.position.z = 2.5;

      const renderer = new THREE.WebGLRenderer({ antialias: true });
      renderer.setSize(container.clientWidth, container.clientHeight);
      container.appendChild(renderer.domElement);

      const geometry = new THREE.TorusKnotGeometry(0.7, 0.22, 128, 24);
      const material = new THREE.MeshStandardMaterial({ color: "#22c55e", metalness: 0.4, roughness: 0.35 });
      const mesh = new THREE.Mesh(geometry, material);
      scene.add(mesh);

      const directional = new THREE.DirectionalLight("#ffffff", 2.2);
      directional.position.set(2, 2, 4);
      scene.add(directional);

      const ambient = new THREE.AmbientLight("#ffffff", 0.6);
      scene.add(ambient);

      let frameId = 0;
      const animate = () => {
        mesh.rotation.x += 0.0075;
        mesh.rotation.y += 0.01;
        renderer.render(scene, camera);
        frameId = requestAnimationFrame(animate);
      };
      animate();

      const resize = () => {
        const width = container.clientWidth;
        const height = container.clientHeight;
        camera.aspect = width / height;
        camera.updateProjectionMatrix();
        renderer.setSize(width, height);
      };

      window.addEventListener("resize", resize);

      return () => {
        cancelAnimationFrame(frameId);
        window.removeEventListener("resize", resize);
        container.removeChild(renderer.domElement);
        geometry.dispose();
        material.dispose();
        renderer.dispose();
      };
    } catch {
      setRenderError("WebGL not available in this environment.");
      return;
    }
  }, []);

  return (
    <div className="viewer" ref={containerRef} data-testid="three-viewer">
      {renderError ? <p>{renderError}</p> : null}
    </div>
  );
};
