import argparse
import sys
from pathlib import Path

import bpy
from mathutils import Vector


def clear_scene():
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete()


def center_and_ground(objects):
    meshes = [obj for obj in objects if obj.type == "MESH"]
    if not meshes:
        raise RuntimeError("No mesh objects were imported.")

    mins = Vector((float("inf"), float("inf"), float("inf")))
    maxs = Vector((float("-inf"), float("-inf"), float("-inf")))
    for obj in meshes:
        for corner in obj.bound_box:
            world = obj.matrix_world @ Vector(corner)
            mins.x = min(mins.x, world.x)
            mins.y = min(mins.y, world.y)
            mins.z = min(mins.z, world.z)
            maxs.x = max(maxs.x, world.x)
            maxs.y = max(maxs.y, world.y)
            maxs.z = max(maxs.z, world.z)

    center = (mins + maxs) * 0.5
    size = max(maxs.x - mins.x, maxs.y - mins.y, maxs.z - mins.z)
    scale = 2.0 / size if size > 0 else 1.0

    for obj in meshes:
        obj.location -= center
        obj.location.z -= mins.z - center.z
        obj.scale *= scale

    return meshes


def apply_preview_material(meshes):
    mat = bpy.data.materials.new("星湾镇_Modly试件_暖木预览")
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    bsdf.inputs["Base Color"].default_value = (0.72, 0.47, 0.25, 1.0)
    bsdf.inputs["Roughness"].default_value = 0.64
    for obj in meshes:
        obj.data.materials.clear()
        obj.data.materials.append(mat)


def setup_camera_and_light():
    bpy.ops.object.light_add(type="AREA", location=(0, -3, 4))
    light = bpy.context.object
    light.name = "星湾镇_预览柔光"
    light.data.energy = 500
    light.data.size = 4

    bpy.ops.object.camera_add(location=(4.2, -6.4, 3.2), rotation=(1.08, 0, 0.56))
    camera = bpy.context.object
    camera.data.lens = 35
    bpy.context.scene.camera = camera


def render(out_path: Path):
    engines = {item.identifier for item in bpy.context.scene.render.bl_rna.properties["engine"].enum_items}
    bpy.context.scene.render.engine = "BLENDER_EEVEE_NEXT" if "BLENDER_EEVEE_NEXT" in engines else "BLENDER_EEVEE"
    bpy.context.scene.render.resolution_x = 1400
    bpy.context.scene.render.resolution_y = 1000
    bpy.context.scene.world.color = (0.98, 0.96, 0.92)
    bpy.context.scene.render.filepath = str(out_path)
    bpy.ops.render.render(write_still=True)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--mesh", required=True, type=Path)
    parser.add_argument("--out", required=True, type=Path)
    parser.add_argument("--fbx-out", type=Path)
    argv = sys.argv[sys.argv.index("--") + 1 :] if "--" in sys.argv else sys.argv[1:]
    args = parser.parse_args(argv)

    clear_scene()
    bpy.ops.import_scene.gltf(filepath=str(args.mesh))
    imported = list(bpy.context.selected_objects)
    meshes = center_and_ground(imported)
    apply_preview_material(meshes)
    setup_camera_and_light()
    if args.fbx_out:
        args.fbx_out.parent.mkdir(parents=True, exist_ok=True)
        bpy.ops.export_scene.fbx(filepath=str(args.fbx_out), use_selection=False)
    args.out.parent.mkdir(parents=True, exist_ok=True)
    render(args.out)


if __name__ == "__main__":
    main()
