import beamos_structuralanalysis as bsa
import uuid

def test_model():
    builder = bsa.BeamOsDynamicModel(
        uuid.UUID("bb993099-3b48-4311-bddb-681f015da825"))
